using devmon_library.Core;
using devmon_library.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
            _logger.Info("***PingerJob is executing ***");
            IAppSettings appSettings = new AppSettings("appSettings.json");
            IJsonSerializer jsonSerializer = new Core.JsonSerializer();
            IRestClientFactory restClientFactory = new RestClientFactory();
            var serverConnector = new ServerConnector(
                                                    null,
                                                    appSettings,
                                                    jsonSerializer,
                                                    restClientFactory);
            var response = serverConnector.SendPing()?.Result;
            if (response == null)
            {
                _logger.Error("*** PingerJob is aborted ***");
                return;
            }
            var body = response.Content;
            var commands = JsonConvert.DeserializeObject<List<CommandInfo>>(body);
            _logger.Debug("**** CommandInfo List ****");
            _logger.Debug(JsonConvert.SerializeObject(commands));
            var pingResults = await ProcessTasksAsync(commands); //.ConfigureAwait(false);

            if (
                (pingResults != null) && (pingResults.Count > 0))
            {
                await serverConnector.Send(pingResults);
            }
            _logger.Info("*** PingerJob is finished ***");
            await Console.Out.WriteLineAsync("PingerJob is finished.");
        }

        private async Task<ResultInfo> RunTask(CommandInfo commandInfo)
        {
            await Console.Out.WriteLineAsync($"Command: {commandInfo.Type}");
            _logger.Info($"*** RunTask() Started, Command: {commandInfo.Type} ***");
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
                case "url.check":
                    return await UrlCheck(commandInfo);
                case "os.processes":
                case "os.uptime":
                case "os.pendingupdates":
                case "os.lastupdated":
                case "os.idletime":
                    return await OsTask(commandInfo);
                default:
                    return null;
            }
        }

        private static async Task<ResultInfo> MemTask(CommandInfo commandInfo)
        {
            _logger.Info("*** Starting MemTask() ***");
            ResultInfo pingResultInfo = null;
            try
            {
                var memoryCollector = new MemoryCollector(null);
                var memoryUtilization = await memoryCollector.ReadMemoryUtilization();
                pingResultInfo = new ResultInfo(
                                           commandInfo.MonitorCommandId,
                                           (memoryUtilization.FreeBytes / OneGb).ToString("N1"));
            }
            catch (Exception ex)
            {
                _logger.Debug($"MemTask(): {ex.Message}");
            }
            _logger.Info("*** Ending MemTask() ***");
            return pingResultInfo;
        }

        private static async Task<ResultInfo> CpuTask(CommandInfo commandInfo)
        {
            _logger.Info("*** Starting CpuTask() ***");
            ResultInfo pingResultInfo = null;
            try
            {
                var cpuCollector = new CpuCollector(null);
                var cpuUtilization = await cpuCollector.ReadCpuUtilization();

                pingResultInfo = new ResultInfo(
                                            commandInfo.MonitorCommandId,
                                            cpuUtilization.LoadPercentage.ToString("N2"));
            }
            catch (Exception ex)
            {
                _logger.Debug($"CpuTask(): {ex.Message}");
            }
            _logger.Info("*** Ending CpuTask() ***");
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
            _logger.Info("*** Starting OsTask() ***");
            ResultInfo pingResultInfo = null;
            try
            {
                var osCollector = new OsCollector(null, _logger);
                var osUtilization = await osCollector.ReadOsUtilization();
                var value = string.Empty;
                //var unit = string.Empty;
                switch (commandInfo.Type)
                {
                    case "os.processes":
                        value = osUtilization.Processes.ToString();
                        break;
                    case "os.uptime":
                        var output = ToHumanReadableString(osUtilization.UpTime);
                        var parts = output.Split(' ');
                        value = parts[0];
                        break;
                    case "os.idletime":
                        value = osUtilization.IdleTime.ToString();
                        break;
                }

                pingResultInfo = new ResultInfo(
                                            commandInfo.MonitorCommandId,
                                            value);
            }
            catch (Exception ex)
            {
                _logger.Debug($"OsTask(): {ex.Message}");
            }
            _logger.Info("*** Ending OsTask() ***");
            return pingResultInfo;
        }

        private static async Task<ResultInfo> DriveTask(CommandInfo commandInfo)
        {
            _logger.Info("*** Starting DriveTask() ***");
            ResultInfo pingResultInfo = null;
            try
            {
                var driveCollector = new DriveCollector(null);
                var driveUtilizations = await driveCollector.ReadDriveUtilization();

                pingResultInfo = new ResultInfo()
                {
                    Id = commandInfo.MonitorCommandId,
                    //Unit = "gb"
                };

                foreach (var driveUtilization in driveUtilizations)
                {
                    if (commandInfo.Arg1 == driveUtilization.Name)
                    {
                        pingResultInfo.Value =
                            driveUtilization.FreeBytes == null
                            ? "-1"
                            : ((ulong)(driveUtilization.FreeBytes / OneGb)).ToString("N1");
                        _logger.Info("*** Ending DriveTask() ***");
                        return pingResultInfo;
                    }
                }
                pingResultInfo.Value = "-2";
                pingResultInfo.IsSuccess = false;
                pingResultInfo.ReturnCode = -2;
                pingResultInfo.ErrorMessage = "Drive does not exist";
            }
            catch (Exception ex)
            {
                _logger.Debug($"DriveTask(): {ex.Message}");
            }
            _logger.Info("*** Ending DriveTask() ***");
            return pingResultInfo;
        }

        private static async Task<ResultInfo> NetworkTask(
                                                    CommandInfo commandInfo)
        {
            _logger.Info("*** Starting NetworkTask() ***");
            var networkCollector = new NetworkCollector(null);
            ResultInfo pingResultInfo = null;
            try
            {
                var networkUtilizations = await networkCollector.ReadNetworkUtilization();
                pingResultInfo = new ResultInfo()
                {
                    Id = commandInfo.MonitorCommandId,
                    //Unit = "kbytes/sec"
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
                        return pingResultInfo;
                    }
                }
                pingResultInfo.Value = "-1";
                pingResultInfo.IsSuccess = false;
                pingResultInfo.ReturnCode = -1;
                pingResultInfo.ErrorMessage = "Name does not match";
            }
            catch (Exception ex)
            {
                _logger.Debug($"NetworkTask(): {ex.Message}");
            }
            _logger.Info("*** Ending NetworkTask() ***");
            return pingResultInfo;
        }

        private static async Task<ResultInfo> UrlTask(CommandInfo commandInfo)
        {
            _logger.Info($"*** Starting UrlTask: {commandInfo.Arg1} ***");
            ResultInfo pingResultInfo = null;
            try
            {

                var watch = System.Diagnostics.Stopwatch.StartNew();
                var result = await sHttpClient.GetAsync(commandInfo.Arg1);
                watch.Stop();
                pingResultInfo = new ResultInfo()
                {
                    Id = commandInfo.MonitorCommandId,
                    IsSuccess = result.IsSuccessStatusCode,
                    Value = watch.ElapsedMilliseconds.ToString(),
                    ReturnCode = (int)result.StatusCode,
                    //Unit = "ms"
                };
                pingResultInfo.ErrorMessage =
                                    (pingResultInfo.IsSuccess)
                                    ? string.Empty
                                    : result.ReasonPhrase;
            }
            catch (Exception ex)
            {
                _logger.Error($"UrlTask(): {ex.Message}");
            }
            _logger.Info($"*** Ending UrlTask: {commandInfo.Arg1} ***");
            return pingResultInfo;
        }
        private static async Task<ResultInfo> UrlCheck(CommandInfo commandInfo)
        {
            _logger.Info($"*** Starting UrlTask: {commandInfo.Arg1} ***");
            _logger.Info($"*** Starting UrlTask: {commandInfo.Arg2} ***");
            ResultInfo pingResultInfo = null;
            var isExists = false;
            var errorMessage = string.Empty;
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var result = await sHttpClient.GetAsync(commandInfo.Arg1);
                watch.Stop();
                if (result.IsSuccessStatusCode)
                {
                    var messageString = await result.Content.ReadAsStringAsync();
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(messageString);
                    string body = doc.DocumentNode.SelectSingleNode("/html/body").InnerText.ToLower();                   
                    var match = Regex.Matches(body, commandInfo.Arg2.ToLower(), RegexOptions.IgnoreCase);
                    if (match.Count > 0)
                    {
                        isExists = true;
                    }
                    else
                    {
                        errorMessage = "Could not match the regular expression";
                    }
                }
                else
                {
                    errorMessage = result.ReasonPhrase;
                }
                pingResultInfo = new ResultInfo()
                {
                    Id = commandInfo.MonitorCommandId,
                    IsSuccess = isExists,
                    Value = watch.ElapsedMilliseconds.ToString(),
                    ReturnCode = (int)result.StatusCode,
                    ErrorMessage = errorMessage
                    //Unit = "ms"
                };
            }
            catch (Exception ex)
            {
                _logger.Error($"UrlCheck(): {ex.Message}");
            }
            _logger.Info($"*** Ending UrlCheck: {commandInfo.Arg1} ***");
            return pingResultInfo;
        }

        private async Task<List<ResultInfo>> ProcessTasksAsync(List<CommandInfo> commandInfos)
        {
            _logger.Info("Starting *** ProcessTasksAsync() ***");
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
            _logger.Info("*** Ending ProcessTasksAsync() ***");
            return pingResults;
        }
    }
}
