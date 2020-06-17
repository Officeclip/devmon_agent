using dev_web_api.BusinessLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web;

namespace dev_web_api
{
    public class MonitorDb
    {
        private readonly string ConnectionString;

        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public MonitorDb()
        {
            _logger.Info("*** Monitordb() ***");
            var dbPath = Path.Combine(HttpRuntime.AppDomainAppPath, "monitor.sqlite");
            ConnectionString = $"Data Source={dbPath}";
            if (!File.Exists(dbPath))
            {
                CreateNewDatabase(dbPath);
            }
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
                        EmailAddress = sqlite_datareader["email-address"].ToString()
                    };
                    user.Password = (sqlite_datareader["password"] == DBNull.Value)
                                ? string.Empty
                                : sqlite_datareader["password"].ToString();
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

        public List<Agent> GetAgents()
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM agents ORDER By agent_id";
            var agents = new List<Agent>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {
                while (sqlite_datareader.Read())
                {
                    var agent = new Agent()
                    {
                        AgentId = Convert.ToInt32(sqlite_datareader["agent_id"]),
                        Guid = sqlite_datareader["guid"].ToString(),
                        MachineName = sqlite_datareader["machine_name"].ToString(),
                        Alias = 
                                sqlite_datareader["alias"] == DBNull.Value
                                ? string.Empty
                                : sqlite_datareader["alias"].ToString()
                    };
                    agent.RegistrationDate =
                                      ConvertToUtcDateTime(
                                                    sqlite_datareader["registration_date"]);
                    agent.LastQueried =
                                      ConvertToUtcDateTime(
                                                    sqlite_datareader["last_queried"]);
                    agent.LastReplyReceived =
                                      ConvertToUtcDateTime(
                                                    sqlite_datareader["last_reply_received"]);

                    agents.Add(agent);
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
                        WarningLimit = Convert.ToInt32(sqlite_datareader["warning_limit"]),
                        ErrorLimit = Convert.ToInt32(sqlite_datareader["error_limit"]),
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
                    Arg2 = sqlite_datareader["arg2"].ToString()
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
                    Unit = sqlite_datareader["unit"].ToString(),
                    ReturnCode = Convert.ToInt32(sqlite_datareader["return_code"]),
                    ErrorMessage = sqlite_datareader["error_message"].ToString()
                };
                monitorValues.Add(MonitorValue);
            }
            sqlite_datareader.Close();
            sqlLiteConn.Close();
            return monitorValues;
        }

        public void UpsertCommand(MonitorCommand monitorCommand)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    INSERT INTO monitorCommands
                    (
                        monitor_command_id,
                        name,
                        type,
                        arg1,
                        arg2
                    )
                    VALUES
                    (
                        {monitorCommand.MonitorCommandId},
                        '{monitorCommand.Name}',
                        '{monitorCommand.Type}',
                        '{monitorCommand.Arg1}'
                        '{monitorCommand.Arg2}'
                    )
                    ON CONFLICT (monitor_command_id)
                    DO update SET 
                            name = ''{monitorCommand.Name}',
                            type = '{monitorCommand.Type}',
                            arg1 = '{monitorCommand.Arg1}',
                            arg2 = '{monitorCommand.Arg2}'"; 
            cmd.ExecuteNonQuery();
            sqlLiteConn.Close();
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
                        unit,
                        value
                    )
                    VALUES
                    (
                        {monitorValue.AgentId},
                        '{monitorValue.MonitorCommandId}',
                        '{monitorValue.ErrorMessage}',
                        {monitorValue.ReturnCode},
                        '{monitorValue.Unit}',
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

        public void UpdateAlias(int agentId, string alias)
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
                cmd.CommandText = $@"
                    update agents SET 
                        alias = '{alias.Replace("'", "''")}'
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

        public void UpDateMonitorValue(MonitorValue monitorValue)
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
                        unit = '{monitorValue.Unit}',
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
                        Unit = sqlite_datareader["unit"].ToString(),
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
                UpDateMonitorValue(monitorValue);
            }
        }

        public void InsertMonitorCommand(MonitorCommand monitorCommand)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    INSERT INTO monitorCommands
                        (name, type, arg1, arg2) 
                    VALUES
                        ('{monitorCommand.Name}', '{monitorCommand.Type}', '{monitorCommand.Arg1}', '{monitorCommand.Arg2}')";
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
                        '{agentResource.StableDeviceJson.Replace("'", "''")}',
                        '{agentResource.LastUpdatedDate}'
                    )
                    ON CONFLICT (agent_id)
                    DO update SET 
                            stable_device_json = '{agentResource.StableDeviceJson.Replace("'", "''")}',
                            last_updated_date = '{agentResource.LastUpdatedDate}'";

            var cmd = new SQLiteCommand(sqlLiteConn)
            {
                CommandText =    sqlQuery         
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
                                        ConvertToUtcDateTime(
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
                serverGuid = (dbOutput??string.Empty).ToString();
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
                userNotification.LastNotified = ConvertToUtcDateTime(
                                                            sqlite_datareader["last_notified"]);
            }
            sqlite_datareader.Close();
            sqlLiteConn.Close();
            return userNotification;
        }

        private DateTime ConvertToUtcDateTime(object dataReaderObject)
        {
            DateTimeOffset result;
            var isValid = DateTimeOffset.TryParse(
                        dataReaderObject.ToString(),
                        out result);
            return (isValid)
                        ? result.UtcDateTime
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
    }
}