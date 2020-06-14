using dev_web_api.BusinessLayer;
using dev_web_api.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
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

        private static bool IsMonitorValueLimitError(
                                            MonitorValue monitorValue,
                                            MonitorCommandLimit monitorCommandLimit)
        {
            if (!monitorCommandLimit.IsLowLimit)
            {
                if (monitorValue.Value > monitorCommandLimit.ErrorLimit)
                {
                    return true;
                }
            }
            else
            {
                if (monitorValue.Value < monitorCommandLimit.ErrorLimit)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsMonitorValueLimitWarning(
                                           MonitorValue monitorValue,
                                           MonitorCommandLimit monitorCommandLimit)
        {
            if (!monitorCommandLimit.IsLowLimit)
            {
                if (monitorValue.Value > monitorCommandLimit.WarningLimit)
                {
                    return true;
                }
            }
            else
            {
                if (monitorValue.Value < monitorCommandLimit.WarningLimit)
                {
                    return true;
                }
            }
            return false;
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
                    if (IsMonitorValueLimitError(monitorValue, monitorCommandLimit))
                    {
                        return "lightcoral";
                    }
                    if (IsMonitorValueLimitWarning(monitorValue, monitorCommandLimit))
                    {
                        return "lightgoldenrodyellow";
                    }
                    //if (!monitorCommandLimit.IsLowLimit)
                    //{
                    //    if (monitorValue.Value > monitorCommandLimit.ErrorLimit)
                    //    {
                    //        return "lightcoral";
                    //    }
                    //    if (monitorValue.Value > monitorCommandLimit.WarningLimit)
                    //    {
                    //        return "lightgoldenrodyellow";
                    //    }
                    //}
                    //else
                    //{
                    //    if (monitorValue.Value < monitorCommandLimit.ErrorLimit)
                    //    {
                    //        return "lightcoral";
                    //    }
                    //    if (monitorValue.Value < monitorCommandLimit.WarningLimit)
                    //    {
                    //        return "lightgoldenrodyellow";
                    //    }
                    //}
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
                monitorTable.Rows[i+1].Cells[0].InnerHtml = agents[i].ScreenName;
            }
        }

        public static bool IsServerGuidValid(string guid)
        {
            return (new MonitorDb()).GetServerGuid() == guid;
        }

        public static MonitorSettings GetMonitorSettings()
        {
            var fileName = "monitorSettings.json";
            var fi = new FileInfo(
                                Path.Combine(
                                        AppDomain.CurrentDomain.BaseDirectory,
                                        fileName));
            if (!fi.Exists)
            {
                throw new FileNotFoundException($"app settings file not found {fileName}");
            }

            var json = File.ReadAllText(fi.FullName);

            var monitorSettings = MonitorSettings.FromJson(json);
            return monitorSettings;
        }

        private static string EmailDescription(
                                    MonitorCommand monitorCommand,
                                    MonitorValue monitorValue)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Monitor limit exceeded for ");
            stringBuilder.Append(monitorCommand.Name);
            if (!string.IsNullOrEmpty(monitorCommand.Arg1)){
                stringBuilder.Append($": {monitorCommand.Arg1}");
            }
            if (!string.IsNullOrEmpty(monitorCommand.Arg2))
            {
                stringBuilder.Append($": {monitorCommand.Arg2}");
            }
            stringBuilder.Append($" - {monitorValue.Value}{monitorValue.Unit}");
            return stringBuilder.ToString();
        }

        public static void SendMonitorLimitEmail(
                                            List<MonitorValue> monitorValues,
                                            List<MonitorCommandLimit> monitorCommandLimits,
                                            List<MonitorCommand> monitorCommands)
        {
            foreach (var monitorValue in monitorValues)
            {
                foreach (var monitorCommandLimit in monitorCommandLimits)
                {
                    if (IsMonitorValueLimitError(monitorValue, monitorCommandLimit))
                    {
                        var userNotification = (new MonitorDb())
                                                        .GetUserNotification(
                                                                1,
                                                                monitorValue.AgentId,
                                                                monitorValue.MonitorCommandId);
                        if (userNotification != null)
                        {
                            if (DateTime.UtcNow.Subtract(
                                                userNotification.LastNotified).Hours > 1)
                            {
                                var monitorCommand =
                                        monitorCommands.Find(x => x.MonitorCommandId == monitorValue.MonitorCommandId);
                                SendEmail(
                                        userNotification.EmailAddress,
                                        "Your monitor limit is exceeded",
                                        EmailDescription(monitorCommand, monitorValue));
                                userNotification.LastNotified = DateTime.UtcNow;
                                (new MonitorDb()).UpdateUserNotification(userNotification);
                            }
                        }
                    }
                    else
                    {
                        (new MonitorDb()).DeleteUserNotification(
                                                            1,
                                                            monitorValue.AgentId,
                                                            monitorValue.MonitorCommandId);
                    }
                }
            }
        }

        public static void SendEmail(
                                string toEmailAddress,
                                string subject,
                                string body)
        {
            var monitorSettings = GetMonitorSettings();
            var smtpClient = new SmtpClient(monitorSettings.Email.Server)
            {
                Port = monitorSettings.Email.Port,
                Credentials = new NetworkCredential(
                                            monitorSettings.Email.Login,
                                            monitorSettings.Email.Password),
                EnableSsl = monitorSettings.Email.IsSsl,
            };

            smtpClient.Send(
                        monitorSettings.Email.FromEmail, 
                        toEmailAddress,
                        subject, 
                        body);
        }

    }
}