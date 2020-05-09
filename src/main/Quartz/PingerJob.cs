using Geheb.DevMon.Agent.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
            var commands = JsonConvert.DeserializeObject<List<CommandInfo>>(body);
            var pingResults = new List<PingResultInfo>();
            foreach (var command in commands)
            {
                if (command.Name == "url")
                {
                    var isSuccess = HttpPing(command.Arg, out long elapsedMs);
                    var pingResult = new PingResultInfo()
                    {
                        Id = command.Id,
                        IsSuccess = isSuccess,
                        MilliSeconds = (int)elapsedMs
                    };
                    pingResults.Add(pingResult);
                }
            }
            if (pingResults.Count > 0)
            {
                await serverConnector.Send(pingResults);
            }
        }

        private bool HttpPing(string url, out long elapsedMs)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var returnValue = true;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Timeout = 10000;
                request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
                request.Method = "HEAD";
                var response = request.GetResponse();
            }
            catch
            {
                returnValue = false;
            }
            finally
            {
                watch.Stop();
                elapsedMs = watch.ElapsedMilliseconds;
            }
            return returnValue;
        }

    }
}
