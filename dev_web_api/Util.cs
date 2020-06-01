using dev_web_api.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace dev_web_api
{
    public static class Util
    {
        public static DataTable GetMonitorTable(
                            List<Agent> agents,
                            List<MonitorCommand> monitorCommands,
                            List<MonitorValue> MonitorValues)
        {
            var table = new DataTable();
            //table.Columns.Add(new DataColumn()); // First column will contain agent name
            foreach (var monitorCommand in monitorCommands)
            {
                var column = new DataColumn(monitorCommand.Name);
                table.Columns.Add(column);
            }
            DataRow dataRow;
            int columnIndex = 0;
            int lastAgentId = 0;
            bool firstTime = true;
            dataRow = table.NewRow();
            foreach (var MonitorValue in MonitorValues)
            {
                if (MonitorValue.AgentId != lastAgentId)
                {
                    if (!firstTime)
                    {
                        table.Rows.Add(dataRow);
                        dataRow = table.NewRow();
                        columnIndex = 0;
                    }
                    else
                    {
                        firstTime = false;
                    }
                    lastAgentId = MonitorValue.AgentId;
                }
                dataRow[columnIndex++] =
                    $"{MonitorValue.Value} {MonitorValue.Unit}";
            }
            table.Rows.Add(dataRow);
            // Now add one more column with the agent name in the beginning
            var agentColumn = new DataColumn();
            table.Columns.Add(agentColumn);
            agentColumn.SetOrdinal(0);
            for (int i=0; i<agents.Count; i++)
            {
                table.Rows[i][0] = agents[i].MachineName;
            }
            return table;
        }
    }
}