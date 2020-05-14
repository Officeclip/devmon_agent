using Geheb.DevMon.Agent.Core;
using Geheb.DevMon.Agent.Models;
using ImTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
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
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
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
                case "memory":
                    return await MemTask(commandInfo);
                case "network":
                    return await NetworkTask(commandInfo);
                case "drive":
                    return await DriveTask(commandInfo);
                case "os":
                    return await OsTask(commandInfo);
                default:
                    return null;
            }
        }

        private static async Task<ResultInfo> MemTask(CommandInfo commandInfo)
        {
            _logger.Debug("Starting MemTask");
            var memoryCollector = new MemoryCollector(null);
            var memoryUtilization = await memoryCollector.ReadMemoryUtilization();
            var pingResultInfo = new ResultInfo(
                                       commandInfo.Id,
                                       memoryUtilization.FreeBytes.ToString(),
                                       "bytes");
            _logger.Debug("Ending MemTask");
            return pingResultInfo;
        }

        private static async Task<ResultInfo> CpuTask(CommandInfo commandInfo)
        {
            _logger.Debug("Starting CpuTask");
            var cpuCollector = new CpuCollector(null);
            var cpuUtilization = await cpuCollector.ReadCpuUtilization();

            var pingResultInfo = new ResultInfo(
                                        commandInfo.Id,
                                        cpuUtilization.LoadPercentage.ToString("N2"),
                                        "%");
            _logger.Debug("Ending CpuTask");
            return pingResultInfo;
        }

        private static async Task<ResultInfo> OsTask(CommandInfo commandInfo)
        {
            _logger.Debug("Starting OsTask");
            var osCollector = new OsCollector(null);
            var osUtilization = await osCollector.ReadOsUtilization();

            var pingResultInfo = new ResultInfo(
                                        commandInfo.Id,
                                        osUtilization.Processes.ToString(),
                                        "");
            _logger.Debug("Ending OsTask");
            return pingResultInfo;
        }

        private static async Task<ResultInfo> DriveTask(CommandInfo commandInfo)
        {
            _logger.Debug("Starting DriveTask");
            var driveCollector = new DriveCollector(null);
            var driveUtilizations = await driveCollector.ReadDriveUtilization();

            var pingResultInfo = new ResultInfo()
            {
                Id = commandInfo.Id,
                Unit = "bytes"
            };

            foreach (var driveUtilization in driveUtilizations)
            {
                if (commandInfo.Arg1 == driveUtilization.Name)
                {
                    pingResultInfo.Value = driveUtilization.FreeBytes.ToString();
                    _logger.Debug("Ending DriveTask");
                    return pingResultInfo;
                }
            }
            pingResultInfo.IsSuccess = false;
            pingResultInfo.ReturnCode = -1;
            pingResultInfo.ErrorMessage = "Name does not match";
            _logger.Debug("Ending DriveTask");
            return pingResultInfo;
        }

        private static async Task<ResultInfo> NetworkTask(
                                                    CommandInfo commandInfo)
        {
            _logger.Debug("Starting NetworkTask");
            var networkCollector = new NetworkCollector(null);
            var networkUtilizations = await networkCollector.ReadNetworkUtilization();
            var pingResultInfo = new ResultInfo()
            {
                Id = commandInfo.Id,
                Unit = "bytes/sec"
            };

            foreach (var networkUtilization in networkUtilizations)
            {
                if (commandInfo.Arg1 == networkUtilization.Name)
                {
                    switch (commandInfo.Arg2)
                    {
                        case "ReceivedBytesPerSeconds":
                            pingResultInfo.Value = networkUtilization.ReceivedBytesPerSecond.ToString();
                            break;
                        case "SentBytesPerSecond":
                            pingResultInfo.Value = networkUtilization.SentBytesPerSecond.ToString();
                            break;
                    }
                    _logger.Debug("Ending NetworkTask");
                    return pingResultInfo;
                }
            }
            pingResultInfo.IsSuccess = false;
            pingResultInfo.ReturnCode = -1;
            pingResultInfo.ErrorMessage = "Name does not match";
            _logger.Debug("Ending NetworkTask");
            return pingResultInfo;
        }

        private static async Task<ResultInfo> UrlTask(CommandInfo commandInfo)
        {
            _logger.Debug("Starting UrlTask");
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
            _logger.Debug("Ending UrlTask");
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
