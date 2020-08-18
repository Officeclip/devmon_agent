using devmon_library;
using NLog;
using System;
using System.Configuration;
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
            
        }

        protected override void OnStop()
        {
            _logger.Info("Method OnStop()");
            _tokenSource.Cancel();
            try
            {
                _processSmsQueueTask.Wait(_tokenSource.Token);
            }
            catch (Exception e)
            {
                _logger.Error($"OnStop: Error: {e.Message}");
            }
        }

        private async Task PingerLoop(CancellationToken token)
        {
            _logger.Info("Method PingerLoop()");
            try
            {
                var staticFrequencyInMins = Convert.ToInt32(ConfigurationManager.AppSettings["StaticFrequencyInMins"]);
                var commandFrequencyInSecs = Convert.ToInt32(ConfigurationManager.AppSettings["CommandFrequencyInSecs"]);

                var pingerJobCount = 0;
                while (!token.IsCancellationRequested)
                {
                    await (new PingerJob()).Execute();
                    _logger.Info($"Pingerloop: PingerJob executed");
                    if (pingerJobCount++ % staticFrequencyInMins == 0)
                    {
                        await (new StaticJob()).Execute();
                        _logger.Info($"Pingerloop: StaticJob executed");
                    }
                    await Task.Delay(commandFrequencyInSecs * 1000, token);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.Error($"PingerLoop: Operation cancelled: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"PingerLoop: Error: {ex.Message}");
            }
        }

    }
}
