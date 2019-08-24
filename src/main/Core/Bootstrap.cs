using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Core
{
    internal sealed class Bootstrap : IDisposable
    {
        readonly IContainer _container = new Container();

        public Bootstrap()
        {
            _container.Register<IAppSettings, AppSettings>(Made.Of(() => new AppSettings(Arg.Index<string>(0)), r => "appSettings.json"));
            _container.Register<ICancellation, Cancellation>(Reuse.Singleton);
            _container.Register<IJsonSerializer, JsonSerializer>();
            _container.Register<IRestClientFactory, RestClientFactory>();

            _container.Register<ICpuCollector, CpuCollector>();
            _container.Register<IDriveCollector, DriveCollector>();
            _container.Register<IMemoryCollector, MemoryCollector>();
            _container.Register<INetworkCollector, NetworkCollector>();
            _container.Register<IOsCollector, OsCollector>();
            _container.Register<StableDeviceCollector>();
            _container.Register<VolatileDeviceCollector>();
            _container.Register<ServerConnector>();

            _container.Register<App>();
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public void Run()
        {
            var app = _container.Resolve<App>();

            app.Run()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
    }
}
