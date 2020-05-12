using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Quartz
{
    public static class JobScheduler
    {
        public static async Task Start()
        {
            // construct a scheduler factory
            NameValueCollection props = new NameValueCollection
            {
                {
                    "quartz.serializer.type", "binary"
                }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);

            // get a scheduler
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.Start();
            Console.WriteLine("Starting Scheduler");

            AddPingerJob(scheduler);
        }

        public static async void AddPingerJob(IScheduler scheduler)
        {
            IJobDetail job = JobBuilder.Create<PingerJob>()
                .WithIdentity("pinger-job", "group")
                .Build();

            // Trigger the job to run now, and then every 60 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("pinger-trigger", "group")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(60)
                    .RepeatForever())
            .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

    }
}
