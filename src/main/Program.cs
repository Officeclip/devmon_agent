using Geheb.DevMon.Agent.Core;
using Geheb.DevMon.Agent.Models;
using NLog;
using RestSharp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
                using (var boot = new Bootstrap())
                {
                    boot.Run();
                }

                return (int)ExitCode.Success;
            }
            catch (OperationCanceledException)
            {
                _logger.Warn("Operation cancelled");
                return (int)ExitCode.Cancelled;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
                return (int)ExitCode.InternalError;
            }
        }
    }
}
