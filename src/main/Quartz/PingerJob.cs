using Geheb.DevMon.Agent.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            var pingResults = await ProcessAppsAsync(commands); //.ConfigureAwait(false);
            //var pingResults = new List<PingResultInfo>();
            //foreach (var command in commands)
            //{
            //    if (command.Name == "url")
            //    {
            //        var isSuccess = HttpPing(command.Arg, out long elapsedMs);
            //        var pingResult = new PingResultInfo()
            //        {
            //            Id = command.Id,
            //            IsSuccess = isSuccess,
            //            MilliSeconds = (int)elapsedMs
            //        };
            //        pingResults.Add(pingResult);
            //    }
            //}
            if (pingResults.Count > 0)
            {
                await serverConnector.Send(pingResults);
            }
        }

        private async Task<PingResultInfo> PingAsync(CommandInfo commandInfo)
        {
            var httpClient = new HttpClient();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var result = await httpClient.GetAsync(commandInfo.Arg);
            watch.Stop();
            var pingResultInfo = new PingResultInfo()
            {
                Id = commandInfo.Id,
                IsSuccess = result.IsSuccessStatusCode,
                MilliSeconds = (int)watch.ElapsedMilliseconds
            };
            return pingResultInfo;
        }


        private async Task<List<PingResultInfo>> ProcessAppsAsync(List<CommandInfo> commandInfos)
        {

            var appListTasks = commandInfos.Select(
                                            commandInfo => PingAsync(commandInfo)).ToList();

            // Wait asynchronously for all of them to finish
            await Task.WhenAll(appListTasks);

            var pingResults = new List<PingResultInfo>();

            foreach (var appList in appListTasks)
            {
                var appListResult = appList.Result;
                pingResults.Add(appListResult);
            }

            return pingResults;
        }

    //private bool HttpPing(string url, out long elapsedMs)
    //    {
    //        var watch = System.Diagnostics.Stopwatch.StartNew();
    //        var returnValue = true;
    //        try
    //        {
    //            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
    //            request.Timeout = 10000;
    //            request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
    //            request.Method = "HEAD";
    //            var response = request.GetResponse();
    //        }
    //        catch
    //        {
    //            returnValue = false;
    //        }
    //        finally
    //        {
    //            watch.Stop();
    //            elapsedMs = watch.ElapsedMilliseconds;
    //        }
    //        return returnValue;
    //    }

    }
}
