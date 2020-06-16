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
using System.Threading;
using System.Threading.Tasks;

namespace devmon_service
{
    public partial class MonitorService : ServiceBase
    {
        IScheduler scheduler;
        public MonitorService()
        {
            InitializeComponent();
             //AddSettings("xxx", "yyy");
       }

        protected override void OnStart(string[] args)
        {
            Trace.WriteLine("Monitor OnStart");
            Trace.WriteLine($"Monitor Directory: {Directory.GetCurrentDirectory()}");
            //Thread.Sleep(15000);
            scheduler = JobScheduler
                                .Start()
                                .ConfigureAwait(false)
                                .GetAwaiter()
                                .GetResult();
        }

        //public void OnDebug()
        //{
        //    OnStart(null);
        //}

        protected override void OnStop()
        {
            Trace.WriteLine("Monitor OnStop");
            JobScheduler.Stop(scheduler);
        }

    }
}
