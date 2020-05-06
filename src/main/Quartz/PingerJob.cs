using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Quartz
{
    /// <summary>
    /// Sends ping to the server every minute
    /// </summary>
    public class PingerJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)

        {
            await Console.Out.WriteLineAsync("PingerJob is executing.");
        }
    }
}
