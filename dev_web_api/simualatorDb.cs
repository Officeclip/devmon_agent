//using dev_web_api.BusinessLayer;
//using NLog;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace dev_web_api
//{
//    public class simualatorDb
//    {

//        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

//        public void ProcessHistoryByFrequency(MonitorValue monitorValue, DateTime dateTime, int frequency)
//        {
//            DateTime dateStart;
//            DateTime dateEnd;
//            var monitorDb = new MonitorDb();
//            //monitorDb.CovertFrequencyToSubtractHrs(dateTime, frequency, out dateStart);
//          //  bool isEntryPresentinDb = monitorDb.isEntryPresent(monitorValue, dateStart, frequency);


//            if (!isEntryPresentinDb)
//            {
//                int frequencyToAvgEntries;
//                switch (frequency)
//                {
//                    case 1:
//                        frequencyToAvgEntries = 0;
//                        break;
//                    case 2:
//                        frequencyToAvgEntries = 1;
//                        break;
//                    default:
//                        throw new Exception("frequency is not supported");
//                }
//                if (frequency == 2)
//                {
//                    //var averageValue = monitorDb.GetAverageValue(monitorValue, dateStart, dateEnd, frequencyToAvgEntries);
//                    //monitorValue.Value = averageValue;
//                }
//                monitorDb.InsertHistory(monitorValue, dateStart, frequency);
//            }
//        }

//        public void InsertBulkData(
//                           bool isrealSimulation = false,
//                           int hours = 24,
//                           int minutes = 60,
//                           int days = 30)
//        {
//            CreateHourData(isrealSimulation, hours);
//            CreateMinuteData(isrealSimulation, minutes);
//        }
//        public void CreateHourData(bool isRealSimulation, int hours)
//        {
//            var monitorDb = new MonitorDb();

//            for (var i = hours; i >= 0; i--)
//            {
//                var monitorValue = new MonitorValue
//                {
//                    AgentId = 1,
//                    MonitorCommandId = 1,
//                    Value = GetRandomNumber(),
//                    ErrorMessage = ""
//                };
//                var date = DateTime.UtcNow.AddHours(-i);
//                if (isRealSimulation)
//                {
//                    monitorDb.DeleteOldHistory(date, 1);
//                    monitorDb.InsertMonitorHistory(monitorValue, date);
//                }
//                else
//                {
//                    monitorDb.InsertHistory(monitorValue, date, 1);
//                }
//            }
//        }

//        public void CreateMinuteData(bool isRealSimulation, int minutes)
//        {
//            var monitorDb = new MonitorDb();
//            for (var i = minutes - 1; i >= 0; i--)
//            {
//                var monitorValue = new MonitorValue
//                {
//                    AgentId = 1,
//                    MonitorCommandId = 1,
//                    Value = GetRandomNumber(),
//                    ErrorMessage = ""
//                };
//                var date = DateTime.UtcNow.AddMinutes(-i);
//                if (isRealSimulation)
//                {
//                    monitorDb.DeleteOldHistory(date, 1);
//                    InsertMonitorHistory(monitorValue, date);
//                }
//                else
//                {
//                    monitorDb.InsertHistory(monitorValue, date, 1);
//                }
//            }
//        }
//        public void InsertMonitorHistory(MonitorValue monitorValue, DateTime dateTime)
//        {
//            _logger.Info("Method InsertMonitorHistory(...)");
//            _logger.Info(ObjectDumper.Dump(monitorValue));
//            //InsertHistory(monitorValue, dateTime, 0);
//            ProcessHistoryByFrequency(monitorValue, dateTime, 1);
//            ProcessHistoryByFrequency(monitorValue, dateTime, 2);
//        }
//        private int GetRandomNumber()
//        {
//            var ticks = (int)DateTime.Now.Ticks;
//            var randomNumber = new Random(ticks);
//            return randomNumber.Next(250, 300);
//        }
//    }
//}