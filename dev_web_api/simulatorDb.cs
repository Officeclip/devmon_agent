using dev_web_api.BusinessLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations.Model;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using static dev_web_api.frequencyEnum;

namespace dev_web_api
{
    public class simulatorDb
    {

        private readonly string ConnectionString;
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public simulatorDb()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["dbString"].ConnectionString;
        }
        public void ProcessHistoryByFrequency(
                                MonitorValue monitorValue,
                                DateTime dateTime,
                                FrequencyTypes frequency)
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
                    case FrequencyTypes.Hours:
                        frequencyToAvgEntries = 0;
                        monitorValue.Value = monitorDb.GetAverageValue(
                                           monitorValue,
                                           frequencyToAvgEntries);
                        break;
                    case FrequencyTypes.Days
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
                            monitorDb.DeleteOldHistory(date, (int)FrequencyTypes.Minutes);
                            ProcessHistoryByFrequency(monitorValue, date, FrequencyTypes.Days);
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
                        monitorDb.InsertHistory(monitorValue, date, FrequencyTypes.Days);
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
                            monitorDb.DeleteOldHistory(date, (int)FrequencyTypes.Hours);
                            ProcessHistoryByFrequency(monitorValue, date, FrequencyTypes.Hours);
                            ProcessHistoryByFrequency(monitorValue, date, FrequencyTypes.Days);
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
                        monitorDb.InsertHistory(monitorValue, date, FrequencyTypes.Hours);
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
                            monitorDb.DeleteOldHistory(date, (int)FrequencyTypes.Minutes);
                            ProcessHistoryByFrequency(monitorValue, date, FrequencyTypes.Hours);
                            ProcessHistoryByFrequency(monitorValue, date, FrequencyTypes.Days);
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
                        monitorDb.InsertHistory(monitorValue, date, FrequencyTypes.Minutes);
                    }
                }
            }
        }
        public void InsertMonitorHistory(MonitorValue monitorValue, DateTime dateTime)
        {
            _logger.Info("Method InsertMonitorHistory(...)");
            _logger.Info(ObjectDumper.Dump(monitorValue));
            //InsertHistory(monitorValue, dateTime, 0);
            ProcessHistoryByFrequency(monitorValue, dateTime, FrequencyTypes.Hours);
            ProcessHistoryByFrequency(monitorValue, dateTime, FrequencyTypes.Days);
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

        public void UpdateHistoryWithCurrentTime()
        {
            var maxDateTime = GetMaximumDateFromHistory();
            var seconds = DateTime.UtcNow.Subtract(maxDateTime).TotalSeconds;
            UpdateHistoryDate((int)Math.Round(seconds));
        }


        public void UpdatAgentsDate()
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn)
            {
                CommandText = $@" 
                        update agents SET
                        last_reply_received = '{DateTime.UtcNow:o}', 
                        last_queried = '{DateTime.UtcNow:o}'"
            };
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }


        private void UpdateHistoryDate(int seconds)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn)
            {
                CommandText = $@"
                    UPDATE history SET date = DATETIME(date, '{seconds} Seconds')"
            };
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        private DateTime GetMaximumDateFromHistory()
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    SELECT MAX(date) FROM history";
            var dbOutput = cmd.ExecuteScalar();
            var maxDateString = (dbOutput ?? string.Empty).ToString();
            var isValid = DateTimeOffset.TryParse(
                        maxDateString,
                        out DateTimeOffset result);
            return result.DateTime;
        }
    }
}
