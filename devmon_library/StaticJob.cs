using devmon_library.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmon_library
{
    public class StaticJob
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public async Task Execute()
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
            catch (Exception ex)
            {
                _logger.Error($"StaticJob.Execute(): {ex.Message}");
                _logger.Error($"Stack Trace: {ex.StackTrace}");
            }
        }

    }
}
