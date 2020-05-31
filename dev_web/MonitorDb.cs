using dev_web.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace dev_web
{
    public class MonitorDb
    {
        private static SQLiteConnection SqlLiteConn = new SQLiteConnection(
            @"Data Source=C:\OfficeClipNew\OpenSource\devmon_agent\monitor.db");

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
                    MachineName = sqlite_datareader["machine_name"].ToString()
                };
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

        public List<MonitorCommandValue> GetMonitorCommandValues()
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            SqlLiteConn.Open();
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
            SqlLiteConn.Close();
            return monitorCommandValues;
        }
    }
}