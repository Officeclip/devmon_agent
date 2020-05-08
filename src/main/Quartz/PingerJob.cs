using Geheb.DevMon.Agent.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Documents;
using JsonSerializer = Geheb.DevMon.Agent.Core.JsonSerializer;

namespace Geheb.DevMon.Agent.Quartz
{
    /// <summary>
    /// Sends ping to the server every minute
    /// </summary>
    public class PingerJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)

        {
            await Console.Out.WriteLineAsync("PingerJob is executing.");
            IAppSettings appSettings = new AppSettings("appSettings.json");
            IJsonSerializer jsonSerializer = new JsonSerializer();
            IRestClientFactory restClientFactory = new RestClientFactory();
            var serverConnector = new ServerConnector(
                                                    null,
                                                    appSettings,
                                                    jsonSerializer,
                                                    restClientFactory);
            var response = serverConnector.SendPing().Result;
            var body = response.Content;
            var pingResults = JsonConvert.DeserializeObject<List<PingResult>>(body);
        }
    }
}
