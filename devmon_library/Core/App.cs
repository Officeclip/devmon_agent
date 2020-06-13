using devmon_library.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    internal sealed class App
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ICancellation _cancellation;
        private readonly StableDeviceCollector _stableDeviceCollector;
        private readonly VolatileDeviceCollector _volatileDeviceCollector;
        private readonly ServerConnector _serverConnector;
        private readonly IJsonSerializer _jsonSerializer;

        public App(
            ICancellation cancellation,
            StableDeviceCollector stableDeviceCollector, 
            VolatileDeviceCollector volatileDeviceCollector,
            ServerConnector serverConnector,
            IJsonSerializer jsonSerializer)
        {
            _cancellation = cancellation;
            _stableDeviceCollector = stableDeviceCollector;
            _volatileDeviceCollector = volatileDeviceCollector;
            _serverConnector = serverConnector;
            _jsonSerializer = jsonSerializer;
        }

        public async Task Run()
        {
            PrintAppInfo();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                _cancellation.Cancel();
            };

            _logger.Info("Read stable device info...");
            StableDeviceInfo stableDeviceInfo = await _stableDeviceCollector.Read();
            _logger.Info(_jsonSerializer.SerializeWithFormatting(stableDeviceInfo));

            _logger.Info("Read volatile device info...");
            VolatileDeviceInfo volatileDeviceInfo = await _volatileDeviceCollector.Read();
            _logger.Info(_jsonSerializer.SerializeWithFormatting(volatileDeviceInfo));

            _logger.Info("Send stable device info ...");
            await _serverConnector.Send(stableDeviceInfo);

            _logger.Info("Send volatile device info ...");
            await _serverConnector.Send(volatileDeviceInfo);
        }

        void PrintAppInfo()
        {
            var asm = Assembly.GetExecutingAssembly();
            var fileVersion = asm.GetName().Version;
            var fileInfo = FileVersionInfo.GetVersionInfo(asm.Location);

            Console.WriteLine(fileInfo.ProductName + " " + fileVersion);
            Console.WriteLine(fileInfo.LegalCopyright);
            Console.WriteLine();
        }
    }
}
