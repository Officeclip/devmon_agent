using dev_web_api.BusinessLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web;
using static dev_web_api.frequencyEnum;

namespace dev_web_api
{
    public class MonitorDb
    {
        private readonly string ConnectionString;

        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public MonitorDb()
        {
            _logger.Info("*** Monitordb() ***");
            //var dbPath = Path.Combine(HttpRuntime.AppDomainAppPath, "monitor.sqlite");
            //ConnectionString = $"Data Source={dbPath}";
            ConnectionString = ConfigurationManager.ConnectionStrings["dbString"].ConnectionString;
            //if (!File.Exists(dbPath))
            //{
            //    CreateNewDatabase(dbPath);
            //}
        }

        private void CreateNewDatabase(string dbPath)
        {
            _logger.Info("*** CreateNewDatabase(...) ***");
            SQLiteConnection.CreateFile(dbPath);
            var sqlPath = Path.Combine(Path.GetDirectoryName(dbPath), @"sql\schema.sql");
            _logger.Info($"sqlPath = {sqlPath}");

            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = Util.ReadFile(sqlPath);
            _logger.Info(cmd.CommandText);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlLiteConn.Close();
            }
        }



        public List<User> GetUsers()
        {
            _logger.Info("*** GetUsers() ***");
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM users ORDER By user_id";
            var users = new List<User>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {
                while (sqlite_datareader.Read())
                {
                    var user = new User()
                    {
                        UserId = Convert.ToInt32(sqlite_datareader["user_id"]),
                        EmailAddress = sqlite_datareader["email_address"].ToString()
                    };
                    user.Password = (sqlite_datareader["password"] == DBNull.Value)
                                ? string.Empty
                                : sqlite_datareader["password"].ToString();
                    user.EmailOptout = (sqlite_datareader["email_opt"] == DBNull.Value)
                                ? false
                                : Convert.ToInt32(sqlite_datareader["email_opt"]) == 1 ? true : false;
                    users.Add(user);
                }
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlite_datareader.Close();
                sqlLiteConn.Close();
            }
            return users;
        }

        public List<Agent> GetEnabledAgents()
        {
            return GetAgents()
                            .OfType<Agent>()
                            .Where(s => s.Enabled == true)
                            .ToList();
        }

        public List<Agent> GetAgents(int agentId = 0)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            if (agentId > 0)
            {
                sqlite_cmd.CommandText = $@"SELECT * FROM agents where agent_id = {agentId} ORDER By agent_id";
            }
            else
            {
                sqlite_cmd.CommandText = "SELECT * FROM agents ORDER By agent_id";
            }
            var agents = new List<Agent>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {
                agents = ExtractAgents(sqlite_datareader);
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlite_datareader.Close();
                sqlLiteConn.Close();
            }
            return agents;
        }

        public List<Agent> GetSelectedAgents(int agentGroupId)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            if (agentGroupId > 0)
            {
                sqlite_cmd.CommandText = $@"select * from agents where agent_id in (select DISTINCT 
                                            a.agent_id from agents a, agent_group_agent ag 
                                            where a.agent_id = ag.agent_id 
                                            and ag.agent_group_id={agentGroupId})";
            }
            var agents = new List<Agent>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {
                agents = ExtractAgents(sqlite_datareader);
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlite_datareader.Close();
                sqlLiteConn.Close();
            }
            return agents;
        }

        private List<Agent> ExtractAgents(SQLiteDataReader sqlite_datareader)
        {
            var agents = new List<Agent>();
            while (sqlite_datareader.Read())
            {
                Agent agent = GetEnabledAgent(sqlite_datareader);

                agents.Add(agent);
            }
            return agents;
        }

        private Agent GetEnabledAgent(SQLiteDataReader sqlite_datareader)
        {
            var agent = new Agent()
            {
                AgentId = Convert.ToInt32(sqlite_datareader["agent_id"]),
                Guid = sqlite_datareader["guid"].ToString(),
                MachineName = sqlite_datareader["machine_name"].ToString(),
                Alias =
                        sqlite_datareader["alias"] == DBNull.Value
                        ? string.Empty
                        : sqlite_datareader["alias"].ToString(),
                Enabled = Convert.ToBoolean(sqlite_datareader["enabled"])
            };
            agent.RegistrationDate =
                              ConvertToDateTime(
                                            sqlite_datareader["registration_date"]);
            agent.LastQueried =
                              ConvertToDateTime(
                                            sqlite_datareader["last_queried"]);
            agent.LastReplyReceived =
                              ConvertToDateTime(
                                            sqlite_datareader["last_reply_received"]);
            return agent;
        }

        public List<Agent> GetAgentsBySelectedGroup(int agentGroupId)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT * from agents where agent_id in(SELECT agent_id from agent_group_agent where agent_group_id={agentGroupId})";
            var agents = new List<Agent>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {

                while (sqlite_datareader.Read())
                {
                    agents = ExtractAgents(sqlite_datareader);
                }

            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlite_datareader.Close();
                sqlLiteConn.Close();
            }
            return agents;
        }

        public List<MonitorCommandLimit> GetMonitorCommandLimits()
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM monitorCommandLimits";
            var monitorCommandLimits = new List<MonitorCommandLimit>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {
                while (sqlite_datareader.Read())
                {
                    var monitorCommandLimit = new MonitorCommandLimit()
                    {
                        Type = sqlite_datareader["type"].ToString(),
                        WarningLimit = ConvertDbToNullableInt(sqlite_datareader["warning_limit"]),
                        ErrorLimit = ConvertDbToNullableInt(sqlite_datareader["error_limit"]),
                        IsLowLimit = Convert.ToBoolean(sqlite_datareader["is_low_limit"])
                    };
                    monitorCommandLimits.Add(monitorCommandLimit);
                }
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlite_datareader.Close();
                sqlLiteConn.Close();
            }
            return monitorCommandLimits;
        }

        private int? ConvertDbToNullableInt(object dataReaderObject)
        {
            if (dataReaderObject == DBNull.Value)
            {
                return null;
            }
            return Convert.ToInt32(dataReaderObject);
        }

        public Agent GetAgentByGuid(string guid)
        {
            _logger.Info("GetAgentByGuid(...)");
            var agents = GetAgents();
            var agent = agents.Find(x => x.Guid == guid);
            return agent;
        }

        public List<MonitorCommand> GetMonitorCommands()
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM monitorCommands order by monitor_command_id";
            var monitorCommands = new List<MonitorCommand>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                var monitorCommand = new MonitorCommand()
                {
                    MonitorCommandId = Convert.ToInt32(sqlite_datareader["monitor_command_id"]),
                    Name = sqlite_datareader["name"].ToString(),
                    Type = sqlite_datareader["type"].ToString(),
                    Arg1 = sqlite_datareader["arg1"].ToString(),
                    Arg2 = sqlite_datareader["arg2"].ToString(),
                    Unit = sqlite_datareader["unit"].ToString()
                };
                monitorCommands.Add(monitorCommand);
            }
            sqlite_datareader.Close();
            sqlLiteConn.Close();

            return monitorCommands;
        }

        public List<MonitorValue> GetMonitorValues()
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM MonitorValues ORDER BY agent_id, monitor_command_id";
            var monitorValues = new List<MonitorValue>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                var MonitorValue = new MonitorValue()
                {
                    AgentId = Convert.ToInt32(sqlite_datareader["agent_id"]),
                    MonitorCommandId = Convert.ToInt32(sqlite_datareader["monitor_command_id"]),
                    Value = Convert.ToDouble(sqlite_datareader["value"]),
                    //Unit = sqlite_datareader["unit"].ToString(),
                    ReturnCode = Convert.ToInt32(sqlite_datareader["return_code"]),
                    ErrorMessage = sqlite_datareader["error_message"].ToString()
                };
                monitorValues.Add(MonitorValue);
            }
            sqlite_datareader.Close();
            sqlLiteConn.Close();
            return monitorValues;
        }

        public void UpdateMonitorCommand(MonitorCommand monitorCommand)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            var strArg1 = EscapeQuote(monitorCommand.Arg1);
            var strArg2 = EscapeQuote(monitorCommand.Arg2);

            cmd.CommandText = $@"
                    UPDATE monitorCommands
                    SET 
                            name = '{monitorCommand.Name}',
                            type = '{monitorCommand.Type}',
                            arg1 = '{strArg1}',
                            arg2 = '{strArg2}',
                            unit = '{monitorCommand.Unit}'
                    WHERE
                        monitor_command_id = {monitorCommand.MonitorCommandId}";
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public void UpsertMonitorCommand(MonitorCommand monitorCommand)
        {
            _logger.Info("Method UpsertMonitorCommand(...)");
            var monitorCommands = GetMonitorCommands();
            var monitorCommandFound = monitorCommands
                                            .Find(x => x.MonitorCommandId == monitorCommand.MonitorCommandId);
            if (monitorCommandFound == null)
            {
                InsertMonitorCommand(monitorCommand);
            }
            else
            {
                UpdateMonitorCommand(monitorCommand);
            }
        }

        public void DeleteOldHistory(DateTime dateTime)
        {
            DeleteOldHistory(dateTime, 0);
            DeleteOldHistory(dateTime, 1);
            DeleteOldHistory(dateTime, 2);
        }

        private int ConvertFrequencyToHour(FrequencyTypes frequency)
        {
            int hours;
            switch (frequency)
            {
                case FrequencyTypes.Minutes:
                    hours = 1;
                    break;
                case FrequencyTypes.Hours:
                    hours = 24;
                    break;
                case FrequencyTypes.Days:
                    //here it is not hours actually it returns the Date
                    hours = 720;
                    break;

                default:
                    throw new Exception("Frequency not supported");
            }

            return hours;
        }
        public void DeleteOldHistory(DateTime dateTime, int frequency)

        {
            TimeSpan timespan = new TimeSpan(
                                          ConvertFrequencyToHour((FrequencyTypes)frequency), 0, 0);
            var dateCutOff = dateTime
                                    .Subtract(
                                        timespan);
            DeleteHistory(dateCutOff, frequency);
        }

        public void DeleteHistory(DateTime dateTime, int frequency)
        {
            _logger.Info("Method DeleteHistory()");
            string strDateTime = dateTime.ToString("o");
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    DELETE FROM history 
                    WHERE date < '{strDateTime}'
                    AND frequency = {frequency}
                    ";
            _logger.Info(cmd.CommandText);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlLiteConn.Close();
            }
        }
        public void DeleteAllHistory()
        {
            _logger.Info("Method DeleteAllHistory()");
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    DELETE FROM history ";
            _logger.Info(cmd.CommandText);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlLiteConn.Close();
            }
        }

        public void InsertMonitorHistory(MonitorValue monitorValue, DateTime dateTime)
        {
            _logger.Info("Method InsertMonitorHistory(...)");
            _logger.Info(ObjectDumper.Dump(monitorValue));
            InsertHistory(monitorValue, dateTime, FrequencyTypes.Minutes);
            ProcessHistoryByFrequency(monitorValue, dateTime, FrequencyTypes.Hours);
            ProcessHistoryByFrequency(monitorValue, dateTime, FrequencyTypes.Days);
        }



        public void InsertHistory(MonitorValue monitorValue, DateTime dateTime, FrequencyTypes frequency)
        {
            using (var sqlLiteConn = new SQLiteConnection(ConnectionString))
            {
                sqlLiteConn.Open();
                SQLiteTransaction transaction;
                transaction = sqlLiteConn.BeginTransaction();
                var cmd = new SQLiteCommand(sqlLiteConn);
                cmd.CommandText = $@"
                    INSERT INTO history
                    (
                        frequency,
                        agent_id,
                        monitor_command_id,
                        date,
                        value
                    )
                    VALUES
                    (
                        {(int)frequency},
                        {monitorValue.AgentId},
                        {monitorValue.MonitorCommandId},
                        '{dateTime:o}',
                        {monitorValue.Value}
                    )";
                _logger.Info(cmd.CommandText);
                try
                {
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    _logger.Error($"Database Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.Error($"General Error: {ex.Message}");
                }
                finally
                {
                    sqlLiteConn.Close();
                }
            }
        }
        public void ProcessHistoryByFrequency(
                                MonitorValue monitorValue,
                                DateTime dateTime,
                                FrequencyTypes frequency)
        {
            DateTime dateStart;
            ConvertFrequencyToSubtractHrs(dateTime, frequency, out dateStart);
            bool isEntryPresentinDb = isEntryPresent(monitorValue, dateStart, frequency);

            if (!isEntryPresentinDb)
            {
                int frequencyToAvgEntries;
                switch (frequency)
                {
                    case FrequencyTypes.Hours:
                        frequencyToAvgEntries = 0;
                        monitorValue.Value = GetAverageValue(
                                           monitorValue,
                                           frequencyToAvgEntries);
                        break;
                    case FrequencyTypes.Days
                    :
                        frequencyToAvgEntries = 1;
                        monitorValue.Value = GetAverageValue(
                                           monitorValue,
                                           frequencyToAvgEntries);
                        break;
                    default:
                        throw new Exception("frequency is not supported");
                }
                InsertHistory(monitorValue, dateStart, frequency);
            }
        }

        public  void ConvertFrequencyToSubtractHrs(DateTime dateTime, FrequencyTypes frequency, out DateTime dateStart)
        {
            switch (frequency)
            {
                case FrequencyTypes.Hours:
                    dateStart = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
                    break;
                case FrequencyTypes.Days:
                    dateStart = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
                    break;
                default:
                    throw new Exception("frequency is not supported");
            }
        }

        public int GetAverageValue(MonitorValue monitorValue, int frequency)
        {
            var averageValue = 0;
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = $@"SELECT IFNULL(AVG(value),0.00) as average_value FROM history
                                    WHERE frequency = {frequency} AND agent_id = {monitorValue.AgentId } 
                                    AND monitor_command_id = { monitorValue.MonitorCommandId }";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                averageValue = Convert.ToInt32(sqlite_datareader["average_value"]);
            }
            sqlite_datareader.Close();
            sqlLiteConn.Close();
            return averageValue;
        }
        public bool isEntryPresent(MonitorValue monitorValue, DateTime dateStartOfHour, FrequencyTypes frequency)
        {
            var existingEntriesCount = -1;
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = $@"SELECT count(*) as entries_count FROM history
                                    WHERE frequency = {(int)frequency} AND agent_id = {monitorValue.AgentId } 
                                    AND monitor_command_id = { monitorValue.MonitorCommandId } AND
                                    date = '{ dateStartOfHour:o}'";
            _logger.Debug("--------Sql Command-------");
            _logger.Debug(sqlite_cmd.CommandText);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                existingEntriesCount = Convert.ToInt32(sqlite_datareader["entries_count"]);
            }
            sqlite_datareader.Close();
            sqlLiteConn.Close();
            return existingEntriesCount > 0 ? true : false;
        }

        public void InsertMonitorValue(MonitorValue monitorValue)
        {
            _logger.Info("Method InsertMonitorValue(...)");
            _logger.Info(ObjectDumper.Dump(monitorValue));
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    INSERT INTO monitorValues
                    ( 
                        agent_id,
                        monitor_command_id,
                        error_message,
                        return_code,
                        value
                    )
                    VALUES
                    (
                        {monitorValue.AgentId},
                        {monitorValue.MonitorCommandId},
                        '{monitorValue.ErrorMessage}',
                        {monitorValue.ReturnCode},
                        {monitorValue.Value}
                    )";
            _logger.Info(cmd.CommandText);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlLiteConn.Close();
            }
        }

        public void UpdateAlias(int agentId, string alias, bool enabled)
        {
            // First get the agentId
            var agents = GetAgents();
            var agent = agents.Find(x => x.AgentId == agentId);
            if (
                (agent != null) &&
                (agent.MachineName != alias))
            {
                var sqlLiteConn = new SQLiteConnection(ConnectionString);
                sqlLiteConn.Open();
                var cmd = new SQLiteCommand(sqlLiteConn);
                var enabledInt = (enabled) ? 1 : 0;
                cmd.CommandText = $@"
                    update agents SET 
                        alias = '{EscapeQuote(alias)}',
                        enabled = {enabledInt}
                    WHERE
                        agent_id = {agentId}";
                _logger.Info(cmd.CommandText);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    _logger.Error($"Database Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.Error($"General Error: {ex.Message}");
                }
                finally
                {
                    sqlLiteConn.Close();
                }
            }
        }

        public void UpdateLastReceivedReply(int agentId)
        {
            _logger.Info("Method UpdateLastReceivedReply(...)");
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    update agents SET 
                        last_reply_received = '{DateTime.UtcNow:o}'
                    WHERE
                        agent_id = {agentId}";
            _logger.Info(cmd.CommandText);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlLiteConn.Close();
            }
        }

        public void UpdateMonitorValue(MonitorValue monitorValue)
        {
            _logger.Info("Method UpDateMonitorValue(...)");
            _logger.Info(ObjectDumper.Dump(monitorValue));
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    update monitorValues SET 
                        error_message = '{monitorValue.ErrorMessage}',
                        return_code = {monitorValue.ReturnCode},
                        value = {monitorValue.Value}
                    WHERE
                        agent_id = {monitorValue.AgentId} AND
                        monitor_command_id = {monitorValue.MonitorCommandId}";
            _logger.Info(cmd.CommandText);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlLiteConn.Close();
            }
        }

        public MonitorValue GetMonitorValue(int agentId, int monitorCommandId)
        {
            _logger.Info("GetMonitorValue(...)");

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);

            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT * FROM monitorValues WHERE agent_id = {agentId} AND monitor_command_id = {monitorCommandId}";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            MonitorValue monitorValue = null;
            try
            {
                while (sqlite_datareader.Read())
                {
                    monitorValue = new MonitorValue()
                    {
                        Value = Convert.ToDouble(sqlite_datareader["value"]),
                        ReturnCode = Convert.ToInt32(sqlite_datareader["return_code"]),
                        ErrorMessage = sqlite_datareader["error_message"].ToString()
                    };
                }
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlite_datareader.Close();
                sqlLiteConn.Close();
            }
            return monitorValue;
        }


        public void UpsertMonitorValue(MonitorValue monitorValue)
        {
            _logger.Info("Method UpsertMonitorValue(...)");
            _logger.Info(ObjectDumper.Dump(monitorValue));
            var monitorValuePresent =
                GetMonitorValue(monitorValue.AgentId, monitorValue.MonitorCommandId);
            if (monitorValuePresent == null)
            {
                InsertMonitorValue(monitorValue);
            }
            else
            {
                UpdateMonitorValue(monitorValue);
            }
            InsertMonitorHistory(monitorValue, DateTime.UtcNow);

        }

        private string EscapeQuote(string var)
        {
            return var.Replace("'", "''");
        }

        public void InsertMonitorCommand(MonitorCommand monitorCommand)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            var strArg1 = EscapeQuote(monitorCommand.Arg1);
            var strArg2 = EscapeQuote(monitorCommand.Arg2);
            cmd.CommandText = $@"
                    INSERT INTO monitorCommands
                        (name, type, arg1, arg2, unit) 
                    VALUES
                        (
                        '{monitorCommand.Name}', 
                        '{monitorCommand.Type}', 
                        '{strArg1}', 
                        '{strArg2}',
                        '{monitorCommand.Unit}')";
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public void InsertUser(User user)
        {
            _logger.Info("*** InsertUser(...) ***");
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    INSERT INTO users
                        (user_id, email_address, password) 
                    VALUES
                        ({user.UserId}, '{user.EmailAddress}', '{user.Password}')";
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public void DeleteAgent(int agentId)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    DELETE FROM agentResources
                    WHERE
                        agent_id = {agentId};
                    DELETE FROM monitorValues
                    WHERE
                        agent_id = {agentId};
                    DELETE FROM userNotifications
                    WHERE
                        agent_id = {agentId};
                    DELETE FROM agents
                    WHERE
                        agent_id = { agentId};";
            ;
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public void DeleteMonitorCommand(int id)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    DELETE FROM monitorValues
                    WHERE
                        monitor_command_id = {id};
                    DELETE FROM monitorCommands
                    WHERE
                        monitor_command_id = {id}";
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public void DeleteAgentGroup(int agenGrpId)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    DELETE FROM agent_groups
                    WHERE
                        agent_group_id = {agenGrpId};
                    DELETE FROM agent_group_agent
                    WHERE
                        agent_group_id = {agenGrpId}";
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public void UpsertAgentResource(
                                        AgentResource agentResource)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var sqlQuery = $@"
                    INSERT INTO agentResources
                    (
                        agent_id,
                        stable_device_json,
                        last_updated_date
                    )
                    VALUES
                    (
                        {agentResource.AgentId},
                        '{EscapeQuote(agentResource.StableDeviceJson)}',
                        '{agentResource.LastUpdatedDate:o}'
                    )
                    ON CONFLICT (agent_id)
                    DO update SET 
                            stable_device_json = '{EscapeQuote(agentResource.StableDeviceJson)}',
                            last_updated_date = '{agentResource.LastUpdatedDate:o}'";

            var cmd = new SQLiteCommand(sqlLiteConn)
            {
                CommandText = sqlQuery
            };
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public AgentResource GetAgentResource(int agentId)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText =
                $"SELECT * FROM agentResources where agent_id = {agentId}";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            AgentResource agentResource = null;
            while (sqlite_datareader.Read())
            {
                agentResource = new AgentResource()
                {
                    AgentId = Convert.ToInt32(sqlite_datareader["agent_id"]),
                    StableDeviceJson = sqlite_datareader["stable_device_json"].ToString(),
                };
                agentResource.LastUpdatedDate =
                                        ConvertToDateTime(
                                                    sqlite_datareader["last_updated_date"]);
            }
            sqlite_datareader.Close();
            sqlLiteConn.Close();
            return agentResource;
        }

        public void UpsertAgent(Agent agent)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            string sqlQuery = $@"
                    -- use upsert https://stackoverflow.com/a/50718957/89256
                    INSERT INTO agents
                    (
                        guid,
                        machine_name,
                        org_id,
                        registration_date,
                        last_queried
                    )
                    VALUES
                    (
                        '{agent.Guid}',
                        '{agent.MachineName}',
                         {agent.OrgId},
                        '{agent.RegistrationDate:o}',
                        '{agent.LastQueried:o}'
                    )
                    ON CONFLICT (guid)
                    DO update SET 
                            machine_name = '{agent.MachineName}',
                            last_queried = '{agent.LastQueried:o}'";
            _logger.Info($"sqlQuery = {sqlQuery}");
            var cmd = new SQLiteCommand(sqlLiteConn)
            {
                CommandText = sqlQuery
            };
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public string GetServerGuid(bool isInsert = false)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    SELECT server_guid FROM groups";
            var dbOutput = cmd.ExecuteScalar();
            string serverGuid;
            if (isInsert && (dbOutput == null))
            {
                serverGuid = Guid.NewGuid().ToString();
                InsertserverGuid(serverGuid);
            }
            else
            {
                serverGuid = (dbOutput ?? string.Empty).ToString();
            }
            sqlLiteConn.Close();
            return serverGuid;
        }

        private void InsertserverGuid(string serverGuid)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    INSERT INTO groups (server_guid) VALUES ('{serverGuid}')";
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public UserNotification GetUserNotification(
                                    int userId,
                                    int agentId,
                                    int monitorCommandId)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText =
                $@"
                    SELECT * FROM 
                        userNotifications un, users u
                    WHERE
                        u.user_id = un.user_id AND
                        un.user_id = {userId} AND 
                        agent_id = {agentId} AND 
                        monitor_command_id = {monitorCommandId}";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            UserNotification userNotification = null;
            while (sqlite_datareader.Read())
            {
                userNotification = new UserNotification()
                {
                    UserId = userId,
                    EmailAddress = sqlite_datareader["email_address"].ToString(),
                    AgentId = agentId,
                    MonitorCommandId = monitorCommandId,
                };
                userNotification.LastNotified = ConvertToDateTime(
                                                            sqlite_datareader["last_notified"]);
            }
            sqlite_datareader.Close();
            sqlLiteConn.Close();
            return userNotification;
        }

        private DateTime ConvertToDateTime(object dataReaderObject)
        {
            DateTimeOffset result;
            var isValid = DateTimeOffset.TryParse(
                        dataReaderObject.ToString(),
                        out result);
            return (isValid)
                        ? result.DateTime
                        : DateTime.MinValue;
        }

        public void InsertUserNotification(UserNotification userNotification)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn)
            {
                CommandText = $@"
                    INSERT INTO 
                        usernotifications 
                        (
                            user_id, 
                            agent_id, 
                            monitor_command_id, 
                            last_notified
                        )
                    VALUES 
                        (
                            {userNotification.UserId}, 
                            {userNotification.AgentId}, 
                            {userNotification.MonitorCommandId}, 
                            '{userNotification.LastNotified:o}'
                        )"
            };
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public void UpdateUserNotification(UserNotification userNotification)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn)
            {
                CommandText = $@"
                    UPDATE
                        userNotifications 
                    SET
                        last_notified = '{userNotification.LastNotified:o}'
                    WHERE
                        user_id = {userNotification.UserId} AND
                        agent_id = {userNotification.AgentId} AND
                        monitor_command_id = {userNotification.MonitorCommandId}"
            };
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public void UpsertUserNotification(UserNotification userNotification)
        {
            var matchedUserNotification = GetUserNotification(
                                                        userNotification.UserId,
                                                        userNotification.AgentId,
                                                        userNotification.MonitorCommandId);
            if (matchedUserNotification == null)
            {
                InsertUserNotification(userNotification);
            }
            else
            {
                UpdateUserNotification(userNotification);
            }
        }

        public void DeleteUserNotification(
                                    int userId,
                                    int agentId,
                                    int monitorCommandId)
        {
            _logger.Info($"DeleteNotification ({userId}, {agentId}, {monitorCommandId})");
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn)
            {
                CommandText = $@"
                    DELETE FROM
                        userNotifications 
                    WHERE
                        user_id = {userId} AND
                        agent_id = {agentId} AND
                        monitor_command_id = {monitorCommandId}"
            };
            _logger.Info(cmd.CommandText);
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

        public void InsertMonitorCommandLimit(MonitorCommandLimit monitorCommandLimit)
        {
            _logger.Info("Method InsertMonitorCommandLimit(...)");
            _logger.Info(ObjectDumper.Dump(monitorCommandLimit));
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    INSERT INTO monitorCommandLimits
                    ( 
                        type,
                        org_id,
                        warning_limit,
                        error_limit,
                        is_low_limit
                    )
                    VALUES
                    (
                        '{monitorCommandLimit.Type}',
                        1,
                        {monitorCommandLimit.WarningLimit},
                        {monitorCommandLimit.ErrorLimit},
                        {monitorCommandLimit.IsLowLimit}
                    )";
            _logger.Info(cmd.CommandText);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlLiteConn.Close();
            }
        }

        public void UpdateMonitorCommandLimit(MonitorCommandLimit monitorCommandLimit)
        {
            _logger.Info("Method UpdateMonitorCommandLimit(...)");
            _logger.Info(ObjectDumper.Dump(monitorCommandLimit));
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    UPDATE 
                        monitorCommandLimits
                    SET
                        warning_limit = {monitorCommandLimit.WarningLimit},
                        error_limit = {monitorCommandLimit.ErrorLimit}
                    WHERE
                        type = '{monitorCommandLimit.Type}' AND
                        org_id = 1";
            _logger.Info(cmd.CommandText);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlLiteConn.Close();
            }
        }
        public void UpsertMonitorCommandLimit(MonitorCommandLimit monitorCommandLimit)
        {
            var commandLimits = GetMonitorCommandLimits();
            var commandLimit = commandLimits.Find(x => x.Type == monitorCommandLimit.Type);
            if (commandLimit == null)
            {
                InsertMonitorCommandLimit(monitorCommandLimit);
            }
            else
            {
                UpdateMonitorCommandLimit(monitorCommandLimit);
            }
        }

        public List<ChartLine> GetChart(int monitorCommandId, int frequency)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText =
                $@"
                    SELECT * FROM 
                        agents a, history his
                    WHERE
                        a.agent_id = his.agent_id AND
                        his.monitor_command_id = {monitorCommandId} AND 
                        frequency = {frequency}
                    ORDER BY
                        a.agent_id, his.date";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            var chartLines = new List<ChartLine>();
            ChartLine chartLine = null;
            while (sqlite_datareader.Read())
            {
                var agentId = Convert.ToInt32(sqlite_datareader["agent_id"]);
                var agentName = GetAgentName(sqlite_datareader);
                var date = ConvertToDateTime(sqlite_datareader["date"]);
                var timeUnits = GetTimeUnits(frequency, date);
                var value = Convert.ToInt32(sqlite_datareader["value"]);
                int maxValue = GetMaxValue(frequency);

                chartLine = chartLines.Find(x => x.AgentId == agentId);
                if (chartLine == null)
                {
                    chartLine = new ChartLine(agentId, agentName, maxValue);
                    chartLines.Add(chartLine);
                }
                if (timeUnits < maxValue)
                {
                    chartLine.ChartPoints[timeUnits].Value = value;
                    
                }
            }
            foreach (var chart in chartLines)
            {
                chart.FixChart();
            }
            sqlite_datareader.Close();
            sqlLiteConn.Close();
            return chartLines;
        }

        private static int GetMaxValue(int frequency)
        {
            var maxValue = 0;
            switch (frequency)
            {
                case 0:
                    maxValue = 60;
                    break;
                case 1:
                    maxValue = 24;
                    break;
                case 2:
                    maxValue = 30;
                    break;
            }
            return maxValue;
        }

        private int GetTimeUnits(int frequency, DateTime date)
        {
            var timeUnits = 0;
            var timeSpan = DateTime.UtcNow.Subtract(date);
            switch (frequency)
            {
                case 0:
                    timeUnits = Convert.ToInt32(timeSpan.TotalMinutes);
                    break;
                case 1:
                    timeUnits = Convert.ToInt32(timeSpan.TotalHours);
                    break;
                case 2:
                    timeUnits = Convert.ToInt32(timeSpan.TotalDays);
                    break;
            }

            return timeUnits;
        }

        private static string GetAgentName(SQLiteDataReader sqlite_datareader)
        {
            var machineName = sqlite_datareader["machine_name"].ToString();
            var alias =
                    sqlite_datareader["alias"] == DBNull.Value
                    ? string.Empty
                    : sqlite_datareader["alias"].ToString();
            var agentName = (alias == string.Empty)
                                    ? machineName
                                    : alias;
            return agentName;
        }

        public void InsertAgentGroup(
                                      string groupName)
        {

            using (var sqlLiteConn = new SQLiteConnection(ConnectionString))
            {
                sqlLiteConn.Open();
                SQLiteTransaction transaction;
                transaction = sqlLiteConn.BeginTransaction();
                var cmd = new SQLiteCommand(sqlLiteConn);
                cmd.CommandText = $@"
                    INSERT INTO agent_groups
                    (
                        agent_group_name,
                         org_id
                    )
                    VALUES
                    (
                       '{groupName}',{1})";
                _logger.Info(cmd.CommandText);
                try
                {
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    _logger.Error($"Database Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.Error($"General Error: {ex.Message}");
                }
                finally
                {
                    sqlLiteConn.Close();
                }
            }


        }

        public void AddAgentsIntoAgentGroup(int grpId, int agentId)

        {
            using (var sqlLiteConn = new SQLiteConnection(ConnectionString))
            {
                sqlLiteConn.Open();
                SQLiteTransaction transaction;
                transaction = sqlLiteConn.BeginTransaction();
                var cmd = new SQLiteCommand(sqlLiteConn);
                cmd.CommandText = $@"
                    INSERT INTO agent_group_agent
                    (
                        agent_group_id,
                       agent_id
                    )
                    VALUES
                    (
                       {grpId},
                       {agentId} )";
                _logger.Info(cmd.CommandText);
                try
                {
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    _logger.Error($"Database Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.Error($"General Error: {ex.Message}");
                }
                finally
                {
                    sqlLiteConn.Close();
                }
            }

        }
        public List<AgentGroups> GetAgentGroups(int orgId)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = $@"select * from agent_groups where org_id = {orgId} ";
            var agentGroups = new List<AgentGroups>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {
                while (sqlite_datareader.Read())
                {
                    var agentGroup = new AgentGroups()
                    {
                        AgentGroupId = Convert.ToInt32(sqlite_datareader["agent_group_id"]),
                        OrgId = Convert.ToInt32(sqlite_datareader["org_id"]),
                        AgentGroupName = sqlite_datareader["agent_group_name"].ToString(),
                    };
                    agentGroups.Add(agentGroup);
                }
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlite_datareader.Close();
                sqlLiteConn.Close();
            }
            return agentGroups;
        }

        public List<AgentGroups> GetSelectedAgentOfGroup(int orgId)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = $@"select * from agent_groups where org_id = {orgId} ";
            var agentGroups = new List<AgentGroups>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {
                while (sqlite_datareader.Read())
                {
                    var agentGroup = new AgentGroups()
                    {
                        AgentGroupId = Convert.ToInt32(sqlite_datareader["agent_group_id"]),
                        OrgId = Convert.ToInt32(sqlite_datareader["org_id"]),
                        AgentGroupName = sqlite_datareader["agent_group_name"].ToString(),
                    };
                    agentGroups.Add(agentGroup);
                }
            }
            catch (SQLiteException ex)
            {
                _logger.Error($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"General Error: {ex.Message}");
            }
            finally
            {
                sqlite_datareader.Close();
                sqlLiteConn.Close();
            }
            return agentGroups;
        }


        public void UpdateAgentGroup(AgentGroups agent)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);

            cmd.CommandText = $@"
                    UPDATE agent_groups
                    SET 
					agent_group_name = '{agent.AgentGroupName}'                         
                    WHERE
                      agent_group_id   = {agent.AgentGroupId}";
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }
        private int GetRandomNumber()
        {
            var ticks = (int)DateTime.Now.Ticks;
            var randomNumber = new Random(ticks);
            return randomNumber.Next(250, 300);
        }

        public void InsertEmailOpt(int optValue, int userId)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"update users set email_opt= {optValue} where user_id = {userId}";
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
        }

    }



}