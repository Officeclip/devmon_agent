using dev_web.BusinessLayer;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web
{
    public class MonitorDb
    {
        private static SqliteConnection SqlLiteConn = new SqliteConnection(
            @"Data Source=C:\OfficeClipNew\OpenSource\devmon_agent\monitor.db;Version=3;");

        public List<Agent> GetAgents()
        {
            SqliteDataReader sqlite_datareader;
            SqliteCommand sqlite_cmd;
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
                    Name = sqlite_datareader["name"].ToString()
                };
                agents.Add(agent);
            }
            sqlite_datareader.Close();
            return agents;
        }

        public List<MonitorCommand> GetMonitorCommands()
        {
            SqliteDataReader sqlite_datareader;
            SqliteCommand sqlite_cmd;
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
            return monitorCommands;
        }

        public List<MonitorCommandValue> GetMonitorCommandValues()
        {
            SqliteDataReader sqlite_datareader;
            SqliteCommand sqlite_cmd;
            sqlite_cmd = SqlLiteConn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM monitorCommandValues ORDER BY agent_id, monitor_command_id";
            var monitorCommandValues = new List<MonitorCommandValue>();
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                var monitorCommandValue = new MonitorCommandValue()
                {
                    AgentId = Convert.ToInt32(sqlite_datareader["agent_id"]),
                    MonitorCommandId = Convert.ToInt32(sqlite_datareader["monitor_command_id"]),
                    Value = Convert.ToDouble(sqlite_datareader["value"]),
                    Unit = sqlite_datareader["unit"].ToString(),
                    ReturnCode = Convert.ToInt32(sqlite_datareader["return_code"]),
                    ErrorMessage = sqlite_datareader["error_message"].ToString()
                };
                monitorCommandValues.Add(monitorCommandValue);
            }
            sqlite_datareader.Close();
            return monitorCommandValues;
        }
    }
}