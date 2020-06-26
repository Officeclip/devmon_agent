using devmon_library;
using NLog;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace devmon_service
{
    public partial class MonitorService : ServiceBase
    {
        //IScheduler scheduler;
        Task _processSmsQueueTask;
        CancellationTokenSource _tokenSource;
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public MonitorService()
        {
            InitializeComponent();
       }

        protected override void OnStart(string[] args)
        {
            _logger.Info("Method OnStart()");
            _tokenSource = new CancellationTokenSource();
            _processSmsQueueTask =
                    Task.Run(() => PingerLoop(_tokenSource.Token));
            //Trace.WriteLine("Monitor OnStart");
            //Trace.WriteLine($"Monitor Directory: {Directory.GetCurrentDirectory()}");
            //scheduler = JobScheduler
            //                    .Start()
            //                    .ConfigureAwait(false)
            //                    .GetAwaiter()
            //                    .GetResult();
        }

        protected override void OnStop()
        {
            //Trace.WriteLine("Monitor OnStop");
            //JobScheduler.Stop(scheduler);
            _logger.Info("Method OnStop()");
            _tokenSource.Cancel();
            try
            {
                _processSmsQueueTask.Wait(_tokenSource.Token);
            }
            catch (Exception e)
            {
                _logger.Error($"Error: {e.Message}");
            }
        }

        private async Task PingerLoop(CancellationToken token)
        {
            _logger.Info("Method PingerLoop()");
            try
            {
                var pingerJobCount = 0;
                while (!token.IsCancellationRequested)
                {
                    await (new PingerJob()).Execute();
                    if (pingerJobCount++ % 30 == 0)
                    {
                        await (new StaticJob()).Execute();
                    }
                    await Task.Delay(60 * 1000, token);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.Error($"Operation cancelled: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error: {ex.Message}");
            }
        }

    }
}
