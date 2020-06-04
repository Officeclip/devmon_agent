using Geheb.DevMon.Agent.Core;
using Quartz;
using System;
using System.Threading.Tasks;
using JsonSerializer = Geheb.DevMon.Agent.Core.JsonSerializer;

namespace Geheb.DevMon.Agent.Quartz
{
    /// <summary>
    /// Sends static data to server every hour
    /// </summary>
    public class StaticJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("StaticJob is executing.");
            IAppSettings appSettings = new AppSettings("appSettings.json");
            IJsonSerializer jsonSerializer = new JsonSerializer();
            IRestClientFactory restClientFactory = new RestClientFactory();
            var serverConnector = new ServerConnector(
                                                    null,
                                                    appSettings,
                                                    jsonSerializer,
                                                    restClientFactory);

            var stableCollector =
                        new StableDeviceCollector(
                                    new CpuCollector(null),
                                    new MemoryCollector(null),
                                    new NetworkCollector(null),
                                    new DriveCollector(null),
                                    new OsCollector(null));

            var stableDeviceInfo = stableCollector.Read();

//            await serverConnector.Send(await stableDeviceInfo);
        }
        
    }
}
