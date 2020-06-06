using dev_web_api.BusinessLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace dev_web_api
{
    public class MonitorDb
    {
        private static SQLiteConnection SqlLiteConn = new SQLiteConnection(
            @"Data Source=C:\OfficeClipNew\OpenSource\devmon_agent\monitor.db");

        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public List<Agent> GetAgents()
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            SqlLiteConn.Open();
            sqlite_cmd = SqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM agents ORDER By agent_id";
            var agents = new List<Agent>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
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
                agents.Add(agent);
            }
            sqlite_datareader.Close();
            SqlLiteConn.Close();
            return agents;
        }

        public List<MonitorCommand> GetMonitorCommands()
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            SqlLiteConn.Open();
            sqlite_cmd = SqlLiteConn.CreateCommand();
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
            SqlLiteConn.Close();

            return monitorCommands;
        }

        public List<MonitorValue> GetMonitorValues()
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            SqlLiteConn.Open();
            sqlite_cmd = SqlLiteConn.CreateCommand();
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
            SqlLiteConn.Close();
            return monitorValues;
        }

        public void UpdateMonitorCommand(MonitorCommand monitorCommand)
        {
            SqlLiteConn.Open();
            var cmd = new SQLiteCommand(SqlLiteConn);
            cmd.CommandText = $@"
                    UPDATE monitorCommands
                    SET 
                        name = '{monitorCommand.Name}',
                        type = '{monitorCommand.Type}',
                        arg1 = '{monitorCommand.Arg1}',
                        arg2 = '{monitorCommand.Arg2}'
                    WHERE
                        monitor_command_id = {monitorCommand.MonitorCommandId}";
            cmd.ExecuteNonQuery();
            SqlLiteConn.Close();
        }

        public void UpdateMonitorValue(MonitorValue MonitorValue)
        {
            SqlLiteConn.Open();
            var cmd = new SQLiteCommand(SqlLiteConn);
            cmd.CommandText = $@"
                    UPDATE monitorCommands
                    SET 
                        agent_id = {MonitorValue.AgentId},
                        error_message = '{MonitorValue.ErrorMessage}',
                        return_code = {MonitorValue.ReturnCode},
                        monitor_command_id = '{MonitorValue.MonitorCommandId}',
                        unit = '{MonitorValue.Unit}',
                        value = {MonitorValue.Value},
                        last_updated_date = '{DateTime.UtcNow:o}'
                    WHERE
                        monitor_command_id = {MonitorValue.MonitorCommandId}";
            cmd.ExecuteNonQuery();
            SqlLiteConn.Close();
        }

        public void InsertMonitorCommand(MonitorCommand monitorCommand)
        {
            SqlLiteConn.Open();
            var cmd = new SQLiteCommand(SqlLiteConn);
            cmd.CommandText = $@"
                    INSERT INTO monitorCommands
                        (name, type, arg1, arg2) 
                    VALUES
                        ('{monitorCommand.Name}', '{monitorCommand.Type}', '{monitorCommand.Arg1}', '{monitorCommand.Arg2}')";
            cmd.ExecuteNonQuery();
            SqlLiteConn.Close();
        }

        public void DeleteMonitorCommand(int id)
        {
            SqlLiteConn.Open();
            var cmd = new SQLiteCommand(SqlLiteConn);
            cmd.CommandText = $@"
                    DELETE monitorCommands
                    WHERE
                        monitor_command_id = {id}";
            cmd.ExecuteNonQuery();
            SqlLiteConn.Close();
        }

        public void UpdateAgentResourceHardware(
                                        int agentId, 
                                        string hardwareJson,
                                        string softwareJson)
        {
            SqlLiteConn.Open();
            var cmd = new SQLiteCommand(SqlLiteConn)
            {
                CommandText = $@"
                    UPDATE agentResources
                    SET 
                        hardware_json = '{hardwareJson}',
                        software_json = '{softwareJson}',
                        last_updated_date = '{DateTime.UtcNow:o}'                     
                    WHERE
                        agent_id = {agentId}"
            };
            cmd.ExecuteNonQuery();
            SqlLiteConn.Close();
        }

        public AgentResource GetAgentResource(int agentId)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            SqlLiteConn.Open();
            sqlite_cmd = SqlLiteConn.CreateCommand();
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
            }
            sqlite_datareader.Close();
            SqlLiteConn.Close();
            return agentResource;
        }

        public void InsertAgent(Agent agent)
        {
            SqlLiteConn.Open();
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
            var cmd = new SQLiteCommand(SqlLiteConn)
            {
                CommandText = sqlQuery
            };
            cmd.ExecuteNonQuery();   
            SqlLiteConn.Close();
        }
    }
}