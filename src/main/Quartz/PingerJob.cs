using Geheb.DevMon.Agent.Core;
using Geheb.DevMon.Agent.Models;
using ImTools;
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
            var pingResults = await ProcessTasksAsync(commands); //.ConfigureAwait(false);
            
            if (pingResults.Count > 0)
            {
                await serverConnector.Send(pingResults);
            }
        }

        private async Task<ResultInfo> RunTask(CommandInfo commandInfo)
        {
            await Console.Out.WriteLineAsync($"Command: {commandInfo.Command}");
            switch (commandInfo.Command)
            {
                case "url":
                    return await UrlTask(commandInfo);
                case "cpu":
                    return await CpuTask(commandInfo);
                case "mem":
                    return await MemTask(commandInfo);
                default:
                    return null;
            }
        }

        private static async Task<ResultInfo> MemTask(CommandInfo commandInfo)
        {
            var memoryCollector = new MemoryCollector(null);
            var memoryUtilization = await memoryCollector.ReadMemoryUtilization();
            var pingResultInfo = new ResultInfo()
            {
                Id = commandInfo.Id,
                Value = memoryUtilization.FreeBytes.ToString(),
                Unit = "bytes"
            };
            return pingResultInfo;
        }

        private static async Task<ResultInfo> CpuTask(CommandInfo commandInfo)
        {
            float loadPercentage  = 0;
            using (var cpuTime = new PerformanceCounter(
                                        "Processor", "% Processor Time", "_Total"))
            {
                int i = 0;
                while (i++ < 3) // needs multitple times to calcuate correct value
                {
                    loadPercentage = cpuTime.NextValue();
                    await Task.Delay(1000);
                }
            }
            var pingResultInfo = new ResultInfo()
            {
                Id = commandInfo.Id,
                Value = loadPercentage.ToString("N2"),
                Unit = "%"
            };
            return pingResultInfo;
        }

        private static async Task<ResultInfo> UrlTask(CommandInfo commandInfo)
        {
            var httpClient = new HttpClient();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var result = await httpClient.GetAsync(commandInfo.Arg1);
            watch.Stop();
            var pingResultInfo = new ResultInfo()
            {
                Id = commandInfo.Id,
                IsSuccess = result.IsSuccessStatusCode,
                Value = watch.ElapsedMilliseconds.ToString(),
                ReturnCode = (int)result.StatusCode,
                ErrorMessage = result.ReasonPhrase,
                Unit = "ms"
            };
            return pingResultInfo;
        }

        private async Task<List<ResultInfo>> ProcessTasksAsync(List<CommandInfo> commandInfos)
        {
            var appListTasks = commandInfos.Select(
                                            commandInfo => RunTask(commandInfo)).ToList();

            // Wait asynchronously for all of them to finish
            await Task.WhenAll(appListTasks);

            var pingResults = new List<ResultInfo>();

            foreach (var appList in appListTasks)
            {
                var appListResult = appList.Result;
                pingResults.Add(appListResult);
            }

            return pingResults;
        }
    }
}
