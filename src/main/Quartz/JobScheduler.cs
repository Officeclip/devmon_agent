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
    public class JobScheduler
    {
        //public static async Task Start()
        //{
        //    // construct a scheduler factory
        //    NameValueCollection props = new NameValueCollection
        //    {
        //        { 
        //            "quartz.serializer.type", "binary" 
        //        }
        //    };
        //    StdSchedulerFactory factory = new StdSchedulerFactory(props);

        //    // get a scheduler
        //    IScheduler sched = await factory.GetScheduler();
        //    await sched.Start();

        //    // define the job and tie it to our HelloJob class
        //    IJobDetail job = JobBuilder.Create<PingerJob>()
        //        .WithIdentity("myJob", "group1")
        //        .Build();

        //    // Trigger the job to run now, and then every 60 seconds
        //    ITrigger trigger = TriggerBuilder.Create()
        //        .WithIdentity("myTrigger", "group1")
        //        .StartNow()
        //        .WithSimpleSchedule(x => x
        //            .WithIntervalInSeconds(60)
        //            .RepeatForever())
        //    .Build();

        //    await sched.ScheduleJob(job, trigger);
        //}

    }
}
