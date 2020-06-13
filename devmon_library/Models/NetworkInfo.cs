using System.Linq;
using System.Net.NetworkInformation;

namespace devmon_library.Models
{
    public sealed class NetworkInfo
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string MacAddress { get; set; }
        public string[] UnicastAddresses { get; set; }
        public string[] DnsAddresses { get; set; }
        public string[] DhcpServerAddresses { get; set; }

        public NetworkInfo(NetworkInterface networkInterface)
        {
            Name = networkInterface.Name;
            Model = networkInterface.Description;
            Type = networkInterface.NetworkInterfaceType.ToString();
            MacAddress = string.Join(":", networkInterface.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
            var ipProps = networkInterface.GetIPProperties();
            UnicastAddresses = ipProps.UnicastAddresses.Select(a => a.Address.ToString()).ToArray();
            DnsAddresses = ipProps.DnsAddresses.Select(a => a.ToString()).ToArray();
            DhcpServerAddresses = ipProps.DhcpServerAddresses.Select(a => a.ToString()).ToArray();  
        }

        public NetworkInfo()
        {

        }
    }
}
