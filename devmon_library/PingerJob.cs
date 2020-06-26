using devmon_library.Core;
using devmon_library.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace devmon_library
{
    public class PingerJob
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private static HttpClient sHttpClient = new HttpClient(); //https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        private const int OneKb = 1024;
        private const int OneGb = 1073741824;
        public async Task Execute()

        {
            await Console.Out.WriteLineAsync("PingerJob is executing.");
            _logger.Info("PingerJob is executing.");
            IAppSettings appSettings = new AppSettings("appSettings.json");
            IJsonSerializer jsonSerializer = new Core.JsonSerializer();
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

            if (
                (pingResults != null) && (pingResults.Count > 0))
            {
                await serverConnector.Send(pingResults);
            }
            _logger.Info("PingerJob is finished.");
            await Console.Out.WriteLineAsync("PingerJob is finished.");
        }

        private async Task<ResultInfo> RunTask(CommandInfo commandInfo)
        {
            await Console.Out.WriteLineAsync($"Command: {commandInfo.Type}");
            switch (commandInfo.Type)
            {
                case "url.ping":
                    return await UrlTask(commandInfo);
                case "cpu.percent":
                    return await CpuTask(commandInfo);
                case "memory.free":
                    return await MemTask(commandInfo);
                case "network.specific":
                    return await NetworkTask(commandInfo);
                case "drive.free":
                    return await DriveTask(commandInfo);
                case "os.processes":
                case "os.uptime":
                case "os.pendingupdates":
                case "os.lastupdated":
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
                                       commandInfo.MonitorCommandId,
                                       (memoryUtilization.FreeBytes / OneGb).ToString("N1"),
                                       "gb");
            _logger.Debug("Ending MemTask");
            return pingResultInfo;
        }

        private static async Task<ResultInfo> CpuTask(CommandInfo commandInfo)
        {
            _logger.Debug("Starting CpuTask");
            var cpuCollector = new CpuCollector(null);
            var cpuUtilization = await cpuCollector.ReadCpuUtilization();

            var pingResultInfo = new ResultInfo(
                                        commandInfo.MonitorCommandId,
                                        cpuUtilization.LoadPercentage.ToString("N2"),
                                        "%");
            _logger.Debug("Ending CpuTask");
            return pingResultInfo;
        }

        public static string ToHumanReadableString(TimeSpan t)
        {
            if (t.TotalSeconds <= 1)
            {
                return $@"{t:s\.ff} seconds";
            }
            if (t.TotalMinutes <= 1)
            {
                return $@"{t:%s} seconds";
            }
            if (t.TotalHours <= 1)
            {
                return $@"{t:%m} minutes";
            }
            if (t.TotalDays <= 1)
            {
                return $@"{t:%h} hours";
            }

            return $@"{t:%d} days";
        }

        private static async Task<ResultInfo> OsTask(CommandInfo commandInfo)
        {
            _logger.Debug("Starting OsTask");
            var osCollector = new OsCollector(null);
            var osUtilization = await osCollector.ReadOsUtilization();
            var value = string.Empty;
            var unit = string.Empty;
            switch (commandInfo.Type)
            {
                case "os.processes":
                    value = osUtilization.Processes.ToString();
                    break;
                case "os.uptime":
                    var output = ToHumanReadableString(osUtilization.UpTime);
                    var parts = output.Split(' ');
                    value = parts[0];
                    unit =
                        (parts.Length >= 2)
                        ? parts[1]
                        : string.Empty;
                    break;
                //case "os.pendingupdates":
                //    value = osUtilization.Update.PendingUpdates.ToString();
                //    break;
                //case "os.lastupdated":
                //    var lastUpdated = osUtilization.Update.LastUpdateInstalledAt;
                //    value =
                //        (lastUpdated == null)
                //        ? "unknown"
                //        : ((DateTime)lastUpdated).ToString("yyyy-MM-dd");
                //    break;
            }

            var pingResultInfo = new ResultInfo(
                                        commandInfo.MonitorCommandId,
                                        value,
                                        unit);
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
                Id = commandInfo.MonitorCommandId,
                Unit = "gb"
            };

            foreach (var driveUtilization in driveUtilizations)
            {
                if (commandInfo.Arg1 == driveUtilization.Name)
                {
                    pingResultInfo.Value =
                        driveUtilization.FreeBytes == null
                        ? "-1"
                        : ((ulong)(driveUtilization.FreeBytes / OneGb)).ToString("N1");
                    _logger.Debug("Ending DriveTask");
                    return pingResultInfo;
                }
            }
            pingResultInfo.Value = "-1";
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
                Id = commandInfo.MonitorCommandId,
                Unit = "kbytes/sec"
            };

            foreach (var networkUtilization in networkUtilizations)
            {
                if (commandInfo.Arg1 == networkUtilization.Name)
                {
                    switch (commandInfo.Arg2)
                    {
                        case "ReceivedBytesPerSeconds":
                            pingResultInfo.Value = (networkUtilization.ReceivedBytesPerSecond / OneKb).ToString("N1");
                            break;
                        case "SentBytesPerSecond":
                            pingResultInfo.Value = (networkUtilization.SentBytesPerSecond / OneKb).ToString("N1");
                            break;
                    }
                    _logger.Debug("Ending NetworkTask");
                    return pingResultInfo;
                }
            }
            pingResultInfo.Value = "-1";
            pingResultInfo.IsSuccess = false;
            pingResultInfo.ReturnCode = -1;
            pingResultInfo.ErrorMessage = "Name does not match";
            _logger.Debug("Ending NetworkTask");
            return pingResultInfo;
        }

        private static async Task<ResultInfo> UrlTask(CommandInfo commandInfo)
        {
            _logger.Debug($"Starting UrlTask: {commandInfo.Arg1}");


            var watch = System.Diagnostics.Stopwatch.StartNew();
            var result = await sHttpClient.GetAsync(commandInfo.Arg1);
            watch.Stop();
            var pingResultInfo = new ResultInfo()
            {
                Id = commandInfo.MonitorCommandId,
                IsSuccess = result.IsSuccessStatusCode,
                Value = watch.ElapsedMilliseconds.ToString(),
                ReturnCode = (int)result.StatusCode,
                Unit = "ms"
            };
            pingResultInfo.ErrorMessage =
                                (pingResultInfo.IsSuccess)
                                ? string.Empty
                                : result.ReasonPhrase;
            _logger.Debug($"Ending UrlTask: {commandInfo.Arg1}: {watch.ElapsedMilliseconds} ms");
            return pingResultInfo;
        }

        private async Task<List<ResultInfo>> ProcessTasksAsync(List<CommandInfo> commandInfos)
        {
            var pingResults = new List<ResultInfo>();
            try
            {
                var appListTasks = commandInfos.Select(
                                                commandInfo => RunTask(commandInfo)).ToList();

                // Wait asynchronously for all of them to finish
                await Task.WhenAll(appListTasks);


                foreach (var appList in appListTasks)
                {
                    var appListResult = appList.Result;
                    pingResults.Add(appListResult);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"PingerJob.ProcessTaskAsync: {ex.Message}");
                _logger.Error($"PingerJob.ProcessTaskAsync: {ex.StackTrace}");
                return null;
            }
            return pingResults;
        }
    }
}
