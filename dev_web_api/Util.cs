using dev_web_api.BusinessLayer;
using dev_web_api.Controllers;
using FriendlyTime;
using IP2Location;
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
            if (monitorCommandLimit.ErrorLimit != null)
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
            }
            return false;
        }

        private static bool IsMonitorValueLimitWarning(
                                           MonitorValue monitorValue,
                                           MonitorCommandLimit monitorCommandLimit)
        {
            if (monitorCommandLimit.WarningLimit != null)
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
            }
            return false;
        }

        private static string GetBackgroundCellClass(
                                MonitorValue monitorValue,
                                List<MonitorCommand> monitorCommands,
                                List<MonitorCommandLimit> monitorCommandLimits)
        {
            foreach (var monitorCommandLimit in monitorCommandLimits)
            {
                if (GetType(monitorValue, monitorCommands) == monitorCommandLimit.Type)
                {
                    if (IsMonitorValueLimitError(monitorValue, monitorCommandLimit))
                    {
                        return "errorLimit";
                    }
                    if (IsMonitorValueLimitWarning(monitorValue, monitorCommandLimit))
                    {
                        return "warningLimit";
                    }
                }
            }
            return string.Empty;
        }

        private static bool IsAgentUnavailable(Agent agent)
        {
            return (DateTime.UtcNow - agent.LastReplyReceived).TotalMinutes > 10;
        }

        private static string GetColumnTitleLimit(
                               MonitorCommand monitorCommand,
                               List<MonitorCommandLimit> monitorCommandLimits)
        {
            var returnValue = string.Empty;
            var monitorCommandLimit = monitorCommandLimits
                                                .Find(x => x.Type == monitorCommand.Type);
            if (monitorCommandLimit != null)
            {
                var warningLimit = $"Warning Limit: {monitorCommandLimit.WarningLimit}";
                var errorLimit = $"Error Limit: {monitorCommandLimit.ErrorLimit}";
                returnValue = $"{warningLimit}, {errorLimit}";
            }
            return returnValue;
        }

        private static void SetHeaderRow(
                                HtmlTable monitorTable,
                                List<MonitorCommand> monitorCommands,
                                List<MonitorCommandLimit> monitorCommandLimits)
        {
            var row = new HtmlTableRow();
            monitorTable.Rows.Add(row);
            foreach (var monitorCommand in monitorCommands)
            {
                var cell = new HtmlTableCell("th");
                cell.InnerText = monitorCommand.Name;
                var title = GetColumnTitleLimit(monitorCommand, monitorCommandLimits);
                if (title.Length > 0)
                {
                    cell.InnerHtml =
                        $@"<span style=""border-bottom: 1px dashed black"">{monitorCommand.Name }</span>";
                    cell.Attributes.Add("title", title);
                    cell.Attributes.Add("class", "headerTitle");
                }
                row.Cells.Add(cell);
            }
        }

        private static void SetNotAvailableRow(
                                        HtmlTable monitorTable,
                                        int monitorCommandsLength)
        {
            var row = new HtmlTableRow();
            monitorTable.Rows.Add(row);
            var cell = new HtmlTableCell();
            row.Cells.Add(cell);
            cell.ColSpan = monitorCommandsLength;
            cell.InnerHtml = "Agent Not Available";
        }

        private static void SetAgentRow(
                                HtmlTable monitorTable,
                                Agent agent,
                                List<MonitorCommand> monitorCommands,
                                List<MonitorValue> monitorValues,
                                List<MonitorCommandLimit> monitorCommandLimits)
        {
            var row = new HtmlTableRow();
            monitorTable.Rows.Add(row);
            var monitorValuesForAgent = monitorValues
                                                .FindAll(x => x.AgentId == agent.AgentId);
            foreach (var monitorValue in monitorValuesForAgent)
            {
                var cell = new HtmlTableCell();
                switch (monitorValue.ReturnCode)
                {
                    case -2:
                        cell.InnerHtml = @"<span style=""border-bottom: 1px dashed black"">NA</span>";
                        cell.Attributes.Add("title", monitorValue.ErrorMessage);
                        cell.Attributes.Add("class", "notAvailable");
                        break;
                    case -1:
                        cell.InnerHtml = @"<span style=""border-bottom: 1px dashed black"">Error</span>";
                        cell.Attributes.Add("title", monitorValue.ErrorMessage);
                        cell.Attributes.Add("class", "systemError");
                        break;
                    default:
                        var monitorCommand = monitorCommands
                                                        .Find(x => x.MonitorCommandId == monitorValue.MonitorCommandId);
                        var unit = monitorCommand?.Unit ?? string.Empty;
                        cell.InnerText = $"{monitorValue.Value} {unit}";
                        var cssClass = GetBackgroundCellClass(
                                monitorValue,
                                monitorCommands,
                                monitorCommandLimits);
                        if (cssClass.Length > 0)
                        {
                            cell.Attributes.Add("class", cssClass);
                        }
                        break;

                }
                row.Cells.Add(cell);
            }

        }

        public static void SetupMonitorTable(
                                    HtmlTable monitorTable,
                                    List<Agent> agents,
                                    List<MonitorCommand> monitorCommands,
                                    List<MonitorValue> monitorValues,
                                    List<MonitorCommandLimit> monitorCommandLimits)
        {
            SetHeaderRow(monitorTable, monitorCommands, monitorCommandLimits);
            foreach (var agent in agents)
            {
                if (IsAgentUnavailable(agent))
                {
                    SetNotAvailableRow(monitorTable, monitorCommands.Count);
                }
                else
                {
                    SetAgentRow(
                            monitorTable,
                            agent,
                            monitorCommands,
                            monitorValues,
                            monitorCommandLimits);
                }
            }
            AddFirstColumn(monitorTable, agents);
        }

        private static void AddFirstColumn(HtmlTable monitorTable, List<Agent> agents)
        {
            foreach (HtmlTableRow row in monitorTable.Rows)
            {
                var cell = new HtmlTableCell();
                row.Cells.Insert(0, cell);
            }
            for (int i = 0; i < agents.Count; i++)
            {
                var ipAddress = string.Empty;
                var clientCity = string.Empty;
                if (agents[i].ClientIpAddress != string.Empty && agents[i].ClientCity != string.Empty)
                {
                    ipAddress = $"IP: {agents[i].ClientIpAddress}";
                    clientCity = $"{agents[i].ClientCity},";
                }
                var str = GenerateHtmlString(agents, i, ipAddress, clientCity);
                //   PlaceHolder placeHolder = GenerateHtml(agents, i, ipAddress, clientCity);
                //  monitorTable.Rows[i + 1].Cells[0].Controls.Add(placeHolder);
                monitorTable.Rows[i + 1].Cells[0].InnerHtml = str;
                var title = $"Last Response {agents[i].LastReplyReceived.ToFriendlyDateTime()}";
                monitorTable.Rows[i + 1].Cells[0].Attributes.Add("title", title);
                monitorTable.Rows[i + 1].Cells[0].Attributes.Add("class", "headerTitle");
            }
        }

        private static PlaceHolder GenerateHtml(List<Agent> agents, int i, string ipAddress, string clientCity)
        {
            PlaceHolder ph = new PlaceHolder();
            HtmlGenericControl divItem = new HtmlGenericControl("div");
            divItem.Attributes.Add("class", "outer");

            HtmlGenericControl innerDivItem = new HtmlGenericControl("div");
            innerDivItem.Style.Add("display", "inline-block");
            innerDivItem.Style.Add("width", "90%");


            HtmlGenericControl innerChildDivItem1 = new HtmlGenericControl("div");
            innerChildDivItem1.Style.Add("white-space", "nowrap");
            innerChildDivItem1.InnerHtml = agents[i].ScreenName;
            innerDivItem.Controls.Add(innerChildDivItem1);

            HtmlGenericControl innerChildDivItem2 = new HtmlGenericControl("div");
            innerChildDivItem2.Style.Add("font-size", "small");
            innerChildDivItem2.Style.Add("font-weight", "normal");
            innerChildDivItem2.InnerHtml = $"{ipAddress}</br>{clientCity}";
            innerDivItem.Controls.Add(innerChildDivItem2);
            divItem.Controls.Add(innerDivItem);


            HtmlGenericControl innerDivItem2 = new HtmlGenericControl("div");
            innerDivItem2.Style.Add("display", "inline-block");
            innerDivItem2.Style.Add("vertical-align", "top");

            innerChildDivItem1 = new HtmlGenericControl("div");
            innerChildDivItem1.Attributes.Add("class", "dropdown");
            innerChildDivItem2.Attributes.Add("class", "dots");
            innerChildDivItem2.Attributes.Add("onclick", $"myFunction('myDropdown-{agents[i].AgentId}')");
            innerChildDivItem2.Style.Add("visibility", "hidden");
            HtmlGenericControl innerChildDivItem3 = new HtmlGenericControl("div");
            innerChildDivItem3.ID = $"myDropdown-{agents[i].AgentId}";
            innerChildDivItem3.Attributes.Add("class", $"myDdl-{agents[i].AgentId}");
            innerChildDivItem3.Attributes.Add("class", "dropdown-content");
            HtmlGenericControl hwSpan = new HtmlGenericControl("span");
            HtmlGenericControl swSpan = new HtmlGenericControl("span");
            hwSpan.InnerHtml = "Hardware";
            swSpan.InnerHtml = "Software";
            hwSpan.Attributes.Add("onclick", $"window.open('hardware.aspx?id={agents[i].AgentId}','name','width=600,height=600')");
            swSpan.Attributes.Add("onclick", $"window.open('software.aspx?id={agents[i].AgentId}','name','width=600,height=600')");
            innerChildDivItem3.Controls.Add(hwSpan);
            innerChildDivItem3.Controls.Add(swSpan);
            innerChildDivItem2.Controls.Add(innerChildDivItem3);
            innerChildDivItem1.Controls.Add(innerChildDivItem2);
            innerDivItem2.Controls.Add(innerChildDivItem1);
            divItem.Controls.Add(innerDivItem2);
            ph.Controls.Add(divItem);
            return ph;
        }

        public static string GenerateHtmlString(List<Agent> agents, int i, string ipAddress, string clientCity)
        {
            var str = $@"   
       <div class='outer' style='white-space:nowrap;'>
                                <div style='display:inline-block; width: 90%'>
                                     <div style = 'border-bottom: 1px dashed black'>
                                     <div style = 'white-space:nowrap'>
                                         {agents[i].MachineName}
                                      </div>
                                         </div>

                                      <div style='font-size:small;font-weight:normal'>
                                          {ipAddress}
                                          </br>
                                                {clientCity}
                                           {agents[i].ClientCountry}                                      
                                    </div>
                                </div>
                                <div style ='display:inline-block; vertical-align:top'>
 
                                     <div class='dropdown'>
                                        <div class='dots'
                                             style='visibility:hidden; white-space:normal'
                                             onclick=""myFunction('myDropdown_{agents[i].AgentId}')"">
                                            <div id ='myDropdown_{agents[i].AgentId}' class='dropdown-content'>
                                                <a href=""hardware.aspx?id={agents[i].AgentId}"">Hardware</a>
                                                <a href=""software.aspx?id={agents[i].AgentId}"">Software</a> 
                                              </div>
  
                                          </div>
  
                                      </div>
  
                                  </div>
  
                              </div>";
  
            return str;
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
                                    Agent agent,
                                    MonitorCommand monitorCommand,
                                    MonitorValue monitorValue)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Monitor limit exceeded for ");
            stringBuilder.Append(monitorCommand.Name);
            if (!string.IsNullOrEmpty(monitorCommand.Arg1))
            {
                stringBuilder.Append($": {monitorCommand.Arg1}");
            }
            if (!string.IsNullOrEmpty(monitorCommand.Arg2))
            {
                stringBuilder.Append($": {monitorCommand.Arg2}");
            }
            if (agent != null)
            {
                stringBuilder.Append($" for Agent: ${agent.ScreenName}");
            }
            stringBuilder.Append($" - {monitorValue.Value}{monitorCommand.Unit}");
            return stringBuilder.ToString();
        }

        public static void SendMonitorLimitEmail(
                                            List<Agent> agents,
                                            List<MonitorValue> monitorValues,
                                            List<MonitorCommandLimit> monitorCommandLimits,
                                            List<MonitorCommand> monitorCommands)
        {
            foreach (var monitorValue in monitorValues)
            {
                var monitorCommand =
                        monitorCommands.Find(x => x.MonitorCommandId == monitorValue.MonitorCommandId);
                var agent =
                        agents.Find(x => x.AgentId == monitorValue.AgentId);
                foreach (var monitorCommandLimit in monitorCommandLimits)
                {
                    if (monitorCommandLimit.Type != monitorCommand.Type)
                    {
                        break;
                    }
                    if (IsMonitorValueLimitError(monitorValue, monitorCommandLimit))
                    {
                        var userNotification = (new MonitorDb())
                                                        .GetUserNotification(
                                                                1,
                                                                monitorValue.AgentId,
                                                                monitorValue.MonitorCommandId);
                        if (userNotification == null)
                        {
                            (new MonitorDb()).InsertUserNotification(
                                                            new UserNotification()
                                                            {
                                                                UserId = 1,
                                                                AgentId = monitorValue.AgentId,
                                                                MonitorCommandId = monitorValue.MonitorCommandId,
                                                                LastNotified = DateTime.UtcNow
                                                            });
                            // Again getting the just entered notification as we do not have the email address
                            userNotification = (new MonitorDb())
                                                            .GetUserNotification(
                                                                    1,
                                                                    monitorValue.AgentId,
                                                                    monitorValue.MonitorCommandId);
                        }
                        if (DateTime.UtcNow.Subtract(
                                            userNotification.LastNotified).Hours > 1)
                        {
                            SendEmail(
                                    userNotification.EmailAddress,
                                    EmailSubject(agent, monitorCommand),
                                    EmailDescription(agent, monitorCommand, monitorValue));
                            userNotification.LastNotified = DateTime.UtcNow;
                            (new MonitorDb()).UpdateUserNotification(userNotification);
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

        private static string EmailSubject(Agent agent, MonitorCommand monitorCommand)
        {
            return $"Monitor limit is exceeded ${(agent?.ScreenName) ?? string.Empty}: {monitorCommand.Name}";
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

        public static string ReadFile(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }

        public static string GetIpInfo(string ipAddress, bool isCountry)
        {
            var cityName = string.Empty;
            var countryName = string.Empty;
            IPResult ipResult = new IP2Location.IPResult();
            try
            {
                if (ipAddress != "" && ipAddress != null)
                {
                    var ip2Component = new IP2Location.Component();
                    ip2Component.IPDatabasePath = "C:\\officeclipnew\\opensource\\devmon_agent\\dev_web_api\\App_Data\\IP2LOCATION-LITE-DB3.BIN";
                    ipResult = ip2Component.IPQuery(ipAddress);
                    switch (ipResult.Status.ToString())
                    {
                        case "OK":
                            cityName = ipResult.City;
                            countryName = ipResult.CountryLong;
                            break;
                        case "EMPTY_IP_ADDRESS":
                            throw new Exception("IP Address cannot be blank.");

                        case "INVALID_IP_ADDRESS":
                            throw new Exception("Invalid IP Address.");

                        case "MISSING_FILE":
                            throw new Exception("Invalid Database Path.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                ipResult = null;
            }

            return isCountry == true ? countryName : cityName;
        }

        //public static void AddChartItem(
        //            List<ChartLine> chartLines,
        //            int agentId,
        //            string agentName,
        //            int timeUnits,
        //            int value)
        //{
        //    ChartLine chartLine = null;
        //    chartLine = chartLines.Find(x => x.AgentId == agentId);
        //    if (chartLine == null)
        //    {
        //        chartLine = new ChartLine()
        //        {
        //            AgentId = agentId,
        //            AgentName = agentName
        //        };
        //        chartLines.Add(chartLine);
        //    }
        //    var chartPoint = new ChartPoint()
        //    {
        //        Unit = timeUnits,
        //        Value = value
        //    };
        //    if (chartLine.ChartPoints == null)
        //    {
        //        chartLine.ChartPoints = new List<ChartPoint>();
        //    }
        //    chartLine.ChartPoints.Add(chartPoint);
        //}

        //public static ChartLine FixChart(
        //                        ChartLine chartLine,
        //                        int chartRange)
        //{
        //    chartLine.ChartPoints.RemoveAll(x => x.Unit > chartRange);
        //    var maxMinutes = chartLine.ChartPoints.Max(x => x.Unit);
        //    var newChartPoints = new List<ChartPoint>();
        //    for (int mins = 0; mins <= maxMinutes; mins++)
        //    {
        //        var chartPoint = chartLine.ChartPoints.Find(x => x.Unit == mins);
        //        if (chartPoint == null)
        //        {
        //            chartPoint = new ChartPoint()
        //            {
        //                Unit = mins,
        //                Value = -50
        //            };
        //        }
        //        newChartPoints.Add(chartPoint);
        //    }
        //    var newChartLine = new ChartLine()
        //    {
        //        AgentId = chartLine.AgentId,
        //        AgentName = chartLine.AgentName,
        //        ChartPoints = newChartPoints
        //    };
        //    return newChartLine;
        //}

    }
}