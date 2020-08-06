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
                           int minutes = 0,
                           int hours = 0,
                           int days = 0)
        {
            CreateHourData(isrealSimulation, hours);
            CreateMinuteData(isrealSimulation, minutes);
            CreateDaysData(isrealSimulation, days);
        }

        private void CreateDaysData(bool isRealSimulation, int days)
        {
            var monitorDb = new MonitorDb();
            if (days > 0)
            {
                for (var i = days; i >= 0; i--)
                {
                    var date = DateTime.UtcNow.AddDays(-i);
                    if (isRealSimulation)
                    {
                        var realData = GenerateRandomMonitorValues();
                        foreach (var monitorValue in realData)
                        {
                            monitorDb.DeleteOldHistory(date, (int)FreequecyTypes.Minutes);                           
                            ProcessHistoryByFrequency(monitorValue, date, FreequecyTypes.Days);
                        }
                    }
                    else
                    {
                        var monitorValue = new MonitorValue
                        {
                            AgentId = 1,
                            MonitorCommandId = 1,
                            Value = GetRandomNumber(),
                            ErrorMessage = ""
                        };
                        monitorDb.InsertHistory(monitorValue, date, FreequecyTypes.Days);
                    }
                }
            }
        }
        public List<MonitorValue> GenerateRandomMonitorValues()
        {
            var moniorValues = new List<MonitorValue>();
            for (var i = 0; i < 4; i++)
            {
                if (i > 0)
                {
                    var monitorValue = new MonitorValue
                    {
                        AgentId = i,
                        MonitorCommandId = i,
                        Value = GetRandomNumber(),
                        ErrorMessage = ""
                    };
                    moniorValues.Add(monitorValue);

                }
            }

            return moniorValues;
        }

        public void CreateHourData(bool isRealSimulation, int hours)
        {
            var monitorDb = new MonitorDb();
            if (hours > 0)
            {
                for (var i = hours; i >= 0; i--)
                {
                    var date = DateTime.UtcNow.AddHours(-i);
                    if (isRealSimulation)
                    {
                        var realData = GenerateRandomMonitorValues();
                        foreach (var monitorValue in realData)
                        {
                            monitorDb.DeleteOldHistory(date, (int)FreequecyTypes.Hours);
                            ProcessHistoryByFrequency(monitorValue, date, FreequecyTypes.Hours);
                            ProcessHistoryByFrequency(monitorValue, date, FreequecyTypes.Days);
                        }
                    }
                    else
                    {
                        var monitorValue = new MonitorValue
                        {
                            AgentId = 1,
                            MonitorCommandId = 1,
                            Value = GetRandomNumber(),
                            ErrorMessage = ""
                        };
                        monitorDb.InsertHistory(monitorValue, date, FreequecyTypes.Hours);
                    }
                }
            }

        }

        public void CreateMinuteData(bool isRealSimulation, int minutes)
        {
            var monitorDb = new MonitorDb();
            if (minutes > 0)
            {
                for (var i = minutes; i >= 0; i--)
                {
                    var date = DateTime.UtcNow.AddMinutes(-i);
                    if (isRealSimulation)
                    {
                        var realData = GenerateRandomMonitorValues();
                        foreach (var monitorValue in realData)
                        {
                            monitorDb.DeleteOldHistory(date, (int)FreequecyTypes.Minutes);
                            ProcessHistoryByFrequency(monitorValue, date, FreequecyTypes.Hours);
                            ProcessHistoryByFrequency(monitorValue, date, FreequecyTypes.Days);
                        }
                    }
                    else
                    {
                        var monitorValue = new MonitorValue
                        {
                            AgentId = 1,
                            MonitorCommandId = 1,
                            Value = GetRandomNumber(),
                            ErrorMessage = ""
                        };
                        monitorDb.InsertHistory(monitorValue, date, FreequecyTypes.Minutes);
                    }
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
