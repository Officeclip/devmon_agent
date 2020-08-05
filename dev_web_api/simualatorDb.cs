using dev_web_api.BusinessLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;
using static dev_web_api.frequencyEnum;

namespace dev_web_api
{
    public class simualatorDb
    {

        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public void ProcessHistoryByFrequency(
                                MonitorValue monitorValue,
                                DateTime dateTime,
                                FreequecyTypes frequency)
        {
            DateTime dateStart;
            var monitorDb = new MonitorDb();
            monitorDb.ConvertFrequencyToSubtractHrs(dateTime, frequency, out dateStart);
            bool isEntryPresentinDb = monitorDb.isEntryPresent(monitorValue, dateStart, frequency);

            if (!isEntryPresentinDb)
            {
                int frequencyToAvgEntries;
                switch (frequency)
                {
                    case FreequecyTypes.Hours:
                        frequencyToAvgEntries = 0;
                        monitorValue.Value = monitorDb.GetAverageValue(
                                           monitorValue,
                                           frequencyToAvgEntries);
                        break;
                    case FreequecyTypes.Days
                    :
                        frequencyToAvgEntries = 1;
                        monitorValue.Value = monitorDb.GetAverageValue(
                                           monitorValue,
                                           frequencyToAvgEntries);
                        break;
                    default:
                        throw new Exception("frequency is not supported");
                }
                monitorDb.InsertHistory(monitorValue, dateStart, frequency);
            }
        }

        public void InsertBulkData(
                           bool isrealSimulation = false,
                           int minutes = 60,
                           int hours = 24,
                           int days = 30)
        {
            CreateHourData(isrealSimulation, hours);
            CreateMinuteData(isrealSimulation, minutes);
            CreateDaysData(isrealSimulation, days);
        }

        private void CreateDaysData(bool isrealSimulation, int days)
        {
            var monitorDb = new MonitorDb();

            for (var i = days; i >= 0; i--)
            {
                var monitorValue = new MonitorValue
                {
                    AgentId = 1,
                    MonitorCommandId = 1,
                    Value = GetRandomNumber(),
                    ErrorMessage = ""
                };
                var date = DateTime.UtcNow.AddDays(-i);
                if (isrealSimulation)
                {
                    monitorDb.DeleteOldHistory(date, 1);
                    monitorDb.InsertMonitorHistory(monitorValue, date);
                }
                else
                {
                    monitorDb.InsertHistory(monitorValue, date, FreequecyTypes.Hours);
                }
            }
        }

        public void CreateHourData(bool isRealSimulation, int hours)
        {
            var monitorDb = new MonitorDb();

            for (var i = hours; i >= 0; i--)
            {
                var monitorValue = new MonitorValue
                {
                    AgentId = 1,
                    MonitorCommandId = 1,
                    Value = GetRandomNumber(),
                    ErrorMessage = ""
                };
                var date = DateTime.UtcNow.AddHours(-i);
                if (isRealSimulation)
                {
                    monitorDb.DeleteOldHistory(date, 1);
                    monitorDb.InsertMonitorHistory(monitorValue, date);
                }
                else
                {
                    monitorDb.InsertHistory(monitorValue, date, FreequecyTypes.Hours);
                }
            }
        }

        public void CreateMinuteData(bool isRealSimulation, int minutes)
        {
            var monitorDb = new MonitorDb();
            for (var i = minutes - 1; i >= 0; i--)
            {
                var monitorValue = new MonitorValue
                {
                    AgentId = 1,
                    MonitorCommandId = 1,
                    Value = GetRandomNumber(),
                    ErrorMessage = ""
                };
                var date = DateTime.UtcNow.AddMinutes(-i);
                if (isRealSimulation)
                {
                    monitorDb.DeleteOldHistory(date, 1);
                    InsertMonitorHistory(monitorValue, date);
                }
                else
                {
                    monitorDb.InsertHistory(monitorValue, date, FreequecyTypes.Hours);
                }
            }
        }
        public void InsertMonitorHistory(MonitorValue monitorValue, DateTime dateTime)
        {
            _logger.Info("Method InsertMonitorHistory(...)");
            _logger.Info(ObjectDumper.Dump(monitorValue));
            //InsertHistory(monitorValue, dateTime, 0);
            ProcessHistoryByFrequency(monitorValue, dateTime, FreequecyTypes.Hours);
            ProcessHistoryByFrequency(monitorValue, dateTime, FreequecyTypes.Days);
        }
        private int GetRandomNumber()
        {
            var ticks = (int)DateTime.Now.Ticks;
            var randomNumber = new Random(ticks);
            return randomNumber.Next(250, 300);
        }

        public void DeleteAllHistory()
        {
            var monitorDb = new MonitorDb();
            monitorDb.DeleteAllHistory();
        }
    }
}
