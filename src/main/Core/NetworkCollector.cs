using Geheb.DevMon.Agent.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Core
{
    internal sealed class NetworkCollector : INetworkCollector
    {
        private ICancellation _cancellation;

        public NetworkCollector(ICancellation cancellation)
        {
            _cancellation = cancellation;
        }

        public Task<NetworkInfo[]> ReadNetworkInfo()
        {
            var networks = new List<NetworkInfo>();
            foreach (var ni in GetPublicInterfaces())
            {
                _cancellation.Token.ThrowIfCancellationRequested();
                networks.Add(new NetworkInfo(ni));
            }
            return Task.FromResult(networks.ToArray());
        }

        public async Task<NetworkUtilization[]> ReadNetworkUtilization()
        {
            var networks = new List<NetworkUtilization>();
            long lastBytesSent, lastBytesReceived, bytesSentPerSecond, bytesReceivedPerSecond;

            foreach (var ni in GetPublicInterfaces())
            {
                _cancellation.Token.ThrowIfCancellationRequested();
                
                bytesSentPerSecond = bytesReceivedPerSecond = 0;
                lastBytesSent = lastBytesReceived = 0;

                int i = 0;
                while (i++ < 5)
                {
                    var stat = ni.GetIPStatistics();

                    bytesSentPerSecond = stat.BytesSent - lastBytesSent;
                    bytesReceivedPerSecond = stat.BytesReceived - lastBytesReceived;

                    lastBytesSent = stat.BytesSent;
                    lastBytesReceived = stat.BytesReceived;

                    await Task.Delay(1000, _cancellation.Token);
                }

                networks.Add(new NetworkUtilization
                {
                    Name = ni.Name,
                    ReceivedBytesPerSecond = (ulong)bytesReceivedPerSecond,
                    SentBytesPerSecond = (ulong)bytesSentPerSecond
                });
            }

            return networks.ToArray();
        }

        private IEnumerable<NetworkInterface> GetPublicInterfaces()
        {
            var netInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            return netInterfaces.Where(n =>
                n.OperationalStatus == OperationalStatus.Up &&
                n.SupportsMulticast &&
                n.NetworkInterfaceType != NetworkInterfaceType.Loopback);
        }
    }
}
