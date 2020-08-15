using devmon_library;
using devmon_library.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Geheb.DevMon.Agent
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Task _processSmsQueueTask;
            var _tokenSource = new CancellationTokenSource();
            try
            {
                AddAgentGuid();
                _processSmsQueueTask = 
                    Task.Run(() => PingerLoop(_tokenSource.Token));
                bool goAgain = true;
                while (goAgain)
                {
                    Thread.Sleep(1000);
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task PingerLoop(CancellationToken token)
        {
            try
            {
                var staticFrequencyInMins = Convert.ToInt32(ConfigurationManager.AppSettings["StaticFrequencyInMins"]);
                var commandFrequencyInSecs = Convert.ToInt32(ConfigurationManager.AppSettings["CommandFrequencyInSecs"]);
                var pingerJobCount = 0;
                while (!token.IsCancellationRequested)
                {
                    await (new PingerJob()).Execute();
                    if (pingerJobCount++ % staticFrequencyInMins == 0)
                    {
                        await (new StaticJob()).Execute();
                    }
                    await Task.Delay(commandFrequencyInSecs * 1000, token);
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"Operation cancelled: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                settings["agent_guid"] = Util.GetAgentGuid();
                File.WriteAllText("appSettings.json", settings.ToString());
            }
        }
    }
}
