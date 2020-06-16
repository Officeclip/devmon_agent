using devmon_library.Quartz;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace devmon_service
{
    public partial class MonitorService : ServiceBase
    {
        IScheduler scheduler;
        public MonitorService(string[] args)
        {
            InitializeComponent();
            AddSettings(args);
        }

        protected override void OnStart(string[] args)
        {
            Trace.WriteLine("Monitor OnStart");
            Trace.WriteLine($"Monitor Directory: {Directory.GetCurrentDirectory()}");
            scheduler = JobScheduler
                                .Start()
                                .ConfigureAwait(false)
                                .GetAwaiter()
                                .GetResult();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStop()
        {
            Trace.WriteLine("Monitor OnStop");
            JobScheduler.Stop(scheduler);
        }

        private void AddSettings(string[] args)
        {
            var appFolder = AppDomain.CurrentDomain.BaseDirectory;
            var isFirstTime = false;
            try
            {
                JObject settings = JObject.Parse(
                                            File.ReadAllText(
                                                    $"{appFolder}appSettings.json"));
                if ((string)settings["agent_guid"] == "")
                {
                    settings["agent_guid"] = Guid.NewGuid().ToString();
                    isFirstTime = true;
                }
                if ((string)settings["xxx"] == "")
                {
                    settings["xxx"] = args[0];
                    isFirstTime = true;
                }
                if ((string)settings["yyy"] == "")
                {
                    settings["yyy"] = args[1];
                    isFirstTime = true;
                }
                if (isFirstTime)
                {
                    File.WriteAllText(
                        $"{appFolder}appSettings.json",
                        settings.ToString());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
