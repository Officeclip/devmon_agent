using dev_web_api.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public static class Util
    {
        private static string GetType(
                            MonitorValue monitorValue,
                            List<MonitorCommand> monitorCommands)
        {
            foreach (var monitorCommand in monitorCommands)
            {
                if (monitorCommand.MonitorCommandId == monitorValue.MonitorCommandId)
                {
                    return monitorCommand.Type;
                }
            }
            return null;
        }

        private static string GetBackgroundCellColor(
                                MonitorValue monitorValue,
                                List<MonitorCommand> monitorCommands,
                                List<MonitorCommandLimit> monitorCommandLimits)
        {
            var value = monitorValue.Value;
            foreach (var monitorCommandLimit in monitorCommandLimits)
            {
                if (GetType(monitorValue, monitorCommands) == monitorCommandLimit.Type)
                {
                    if (!monitorCommandLimit.IsLowLimit)
                    {
                        if (monitorValue.Value > monitorCommandLimit.ErrorLimit)
                        {
                            return "lightcoral";
                        }
                        if (monitorValue.Value > monitorCommandLimit.WarningLimit)
                        {
                            return "lightgoldenrodyellow";
                        }
                    }
                    else
                    {
                        if (monitorValue.Value < monitorCommandLimit.ErrorLimit)
                        {
                            return "lightcoral";
                        }
                        if (monitorValue.Value < monitorCommandLimit.WarningLimit)
                        {
                            return "lightgoldenrodyellow";
                        }
                    }
                }
            }
            return string.Empty;
        }

        public static void SetupMonitorTable(
                                    ref HtmlTable monitorTable,
                                    List<Agent> agents,
                                    List<MonitorCommand> monitorCommands,
                                    List<MonitorValue> monitorValues,
                                    List<MonitorCommandLimit> monitorCommandLimits)
        {
            // Setup the Header
            var row = new HtmlTableRow();
            monitorTable.Rows.Add(row);
            var cell = new HtmlTableCell("th");
            row.Cells.Add(cell);
            foreach (var monitorCommand in monitorCommands)
            {
                cell = new HtmlTableCell("th");
                cell.InnerHtml = monitorCommand.Name;
                row.Cells.Add(cell);
            }

            int lastAgentId = 0;
            bool firstTime = true;
            row = new HtmlTableRow();
            monitorTable.Rows.Add(row);
            cell = new HtmlTableCell();
            row.Cells.Add(cell);
            foreach (var monitorValue in monitorValues)
            {
                if (monitorValue.AgentId != lastAgentId)
                {
                    if (!firstTime)
                    {
                        monitorTable.Rows.Add(row);
                        row = new HtmlTableRow();
                        monitorTable.Rows.Add(row);
                        cell = new HtmlTableCell();
                        row.Cells.Add(cell);
                    }
                    else
                    {
                        firstTime = false;
                    }
                    lastAgentId = monitorValue.AgentId;
                }
                cell = new HtmlTableCell();
                cell.BgColor = GetBackgroundCellColor(
                                                monitorValue,
                                                monitorCommands,
                                                monitorCommandLimits);
                cell.InnerHtml = $"{monitorValue.Value} {monitorValue.Unit}";
                row.Cells.Add(cell);
            }
            for (int i = 0; i < agents.Count; i++)
            {
                monitorTable.Rows[i+1].Cells[0].InnerHtml = agents[i].MachineName;
            }
        }

        public static bool IsServerGuidValid(string guid)
        {
            return (new MonitorDb()).GetServerGuid() == guid;
        }

    }
}