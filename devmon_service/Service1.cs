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
    public partial class Service1 : ServiceBase
    {
        IScheduler scheduler;
        public Service1()
        {
            InitializeComponent();
            AddAgentGuid();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Trace.WriteLine(Directory.GetCurrentDirectory());
            scheduler = JobScheduler
                                .Start()
                                .ConfigureAwait(false)
                                .GetAwaiter()
                                .GetResult();
        }

        protected override void OnStop()
        {
            JobScheduler.Stop(scheduler);
        }

        private void AddAgentGuid()
        {
            var appFolder = AppDomain.CurrentDomain.BaseDirectory;
            JObject settings = JObject.Parse(
                                        File.ReadAllText(
                                                $"{appFolder}appSettings.json"));
            if ((string)settings["agent_guid"] == "")
            {
                settings["agent_guid"] = Guid.NewGuid().ToString();
            }
            File.WriteAllText(
                $"{appFolder}appSettings.json", 
                settings.ToString());
        }
    }
}
