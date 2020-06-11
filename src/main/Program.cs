using Geheb.DevMon.Agent.Core;
using Geheb.DevMon.Agent.Models;
using Geheb.DevMon.Agent.Quartz;
using ImTools;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent
{
    class Program
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                _logger.Info("Starting the main program");
                AddAgentGuid();
                while (true)
                {
                    var scheduler = JobScheduler.Start().ConfigureAwait(false);
                    //JobScheduler.Stop(scheduler.GetAwaiter().GetResult());
                    //var isShutdown = JobScheduler.IsShutdown(scheduler.GetAwaiter().GetResult());
                    var cpuInfo = GetCpuInfo().Result;
                    Console.WriteLine(cpuInfo.Name);
                    Thread.Sleep(10000 * 100000);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.Warn("Operation cancelled");
                Console.WriteLine("Operation cancelled");
                return (int)ExitCode.Cancelled;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _logger.Fatal(ex);
                return (int)ExitCode.InternalError;
            }
        }

        /// <summary>
        /// If the agentGuid is not in the appSettings.json file then add it there...
        /// </summary>
        static void AddAgentGuid()
        {
            JObject settings = JObject.Parse(File.ReadAllText("appSettings.json"));
            if ((string)settings["agent_guid"] == "")
            {
                settings["agent_guid"] = Guid.NewGuid().ToString();
            }
            File.WriteAllText("appSettings.json", settings.ToString());
        }

        static Task<CpuInfo> GetCpuInfo()
        {
            return (new CpuCollector(null)).ReadCpuInfo();

        }
    }
}
