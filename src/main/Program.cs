using Geheb.DevMon.Agent.Core;
using Geheb.DevMon.Agent.Models;
using Geheb.DevMon.Agent.Quartz;
using NLog;
using RestSharp;
using System;
using System.Diagnostics;
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
                while (true)
                {
                    JobScheduler.Start().ConfigureAwait(false);
                    //using (var boot = new Bootstrap())
                    //{
                    //    boot.Run();
                    //}

                    var cpuInfo = GetCpuInfo().Result;
                    Console.WriteLine(cpuInfo.Name);
                    Thread.Sleep(10000 * 100000);
                }
                
                //return (int)ExitCode.Success;
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

        static Task<CpuInfo> GetCpuInfo()
        {
            return (new CpuCollector(null)).ReadCpuInfo();

        }
    }
}
