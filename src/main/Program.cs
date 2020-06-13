using devmon_library.Models;
using devmon_library.Quartz;
using Newtonsoft.Json.Linq;
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
        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                AddAgentGuid();
                while (true)
                {
                    var scheduler = JobScheduler.Start().ConfigureAwait(false);
                    //var cpuInfo = GetCpuInfo().Result;
                    //Console.WriteLine(cpuInfo.Name);
                    Thread.Sleep(10000 * 100000);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation cancelled");
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
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
                File.WriteAllText("appSettings.json", settings.ToString());
            }
        }

        //static Task<CpuInfo> GetCpuInfo()
        //{
        //    return (new CpuCollector(null)).ReadCpuInfo();

        //}
    }
}
