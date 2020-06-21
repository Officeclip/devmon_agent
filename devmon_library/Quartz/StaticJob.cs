using devmon_library.Core;
using NLog;
using Quartz;
using System;
using System.Threading.Tasks;
using JsonSerializer = devmon_library.Core.JsonSerializer;

namespace devmon_library.Quartz
{
    /// <summary>
    /// Sends static data to server every hour
    /// </summary>
    public class StaticJob : IJob
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.Info("Start StaticJob.Execute()");
            try
            {
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
                                        new OsCollector(null),
                                        new SoftwareCollector(null));

                var stableDeviceInfo = stableCollector.Read();

                await serverConnector.Send(await stableDeviceInfo);
            }
            catch(Exception ex)
            {
                _logger.Error($"StaticJob.Execute(): {ex.Message}");
                _logger.Error($"Stack Trace: {ex.StackTrace}");
            }
        }

    }
}
