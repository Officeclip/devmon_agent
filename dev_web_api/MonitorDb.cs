using dev_web_api.BusinessLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace dev_web_api
{
    public class MonitorDb
    {
        private const string ConnectionString =
            @"Data Source=C:\OfficeClipNew\OpenSource\devmon_agent\monitor.db";
        //private static SQLiteConnection sqlLiteConn = new SQLiteConnection(ConnectionString);

        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

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
                    };
                    DateTime result;
                    var isValid = DateTime.TryParse(
                                sqlite_datareader["registration_date"].ToString(),
                                out result);
                    if (isValid)
                    {
                        agent.RegistrationDate = result;
                    }
                    isValid = DateTime.TryParse(
                                sqlite_datareader["last_queried"].ToString(),
                                out result);
                    if (isValid)
                    {
                        agent.LastQueried = result;
                    }
                    isValid = DateTime.TryParse(
                                sqlite_datareader["last_reply_received"].ToString(),
                                out result);
                    if (isValid)
                    {
                        agent.LastReplyReceived = result;
                    }
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

        public Agent GetAgentByGuid(string guid)
        {
            _logger.Info("GetAgentByGuid(...)");

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            var sqlLiteConn = new SQLiteConnection(ConnectionString);

            sqlLiteConn.Open();
            sqlite_cmd = sqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT * FROM agents WHERE guid = '{guid}'";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            Agent agent = null;
            try
            {
                while (sqlite_datareader.Read())
                {
                    agent = new Agent()
                    {
                        AgentId = Convert.ToInt32(sqlite_datareader["agent_id"]),
                        Guid = sqlite_datareader["guid"].ToString(),
                        MachineName = sqlite_datareader["machine_name"].ToString(),
                    };
                    DateTime result;
                    var isValid = DateTime.TryParse(
                                sqlite_datareader["registration_date"].ToString(),
                                out result);
                    if (isValid)
                    {
                        agent.RegistrationDate = result;
                    }
                    isValid = DateTime.TryParse(
                                sqlite_datareader["last_queried"].ToString(),
                                out result);
                    if (isValid)
                    {
                        agent.LastQueried = result;
                    }
                    isValid = DateTime.TryParse(
                                sqlite_datareader["last_reply_received"].ToString(),
                                out result);
                    if (isValid)
                    {
                        agent.LastReplyReceived = result;
                    }
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
                        error_message,
                        return_code,
                        monitor_command_id,
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

        public void DeleteMonitorCommand(int id)
        {
            var sqlLiteConn = new SQLiteConnection(ConnectionString);
            sqlLiteConn.Open();
            var cmd = new SQLiteCommand(sqlLiteConn);
            cmd.CommandText = $@"
                    DELETE monitorCommands
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
                        hardware_json,
                        software_json,
                        last_updated_date
                    )
                    VALUES
                    (
                        {agentResource.AgentId},
                        '{agentResource.HardwareJson}',
                        '{agentResource.SoftwareJson}',
                        '{agentResource.LastUpdatedDate}'
                    )
                    ON CONFLICT (agent_id)
                    DO update SET 
                            hardware_json = '{agentResource.HardwareJson}',
                            software_json = '{agentResource.SoftwareJson}',
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
                    HardwareJson = sqlite_datareader["hardware_json"].ToString(),
                    SoftwareJson = sqlite_datareader["software_json"].ToString()
                };
                DateTime result;
                var isValid = DateTime.TryParse(
                            sqlite_datareader["last_updated_date"].ToString(),
                            out result);
                if (isValid)
                {
                    agentResource.LastUpdatedDate = result;
                }
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
    }
}