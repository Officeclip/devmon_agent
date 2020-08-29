using dev_web_api.BusinessLayer;
using dev_web_api.Controllers;
using FriendlyTime;
using IP2Location;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public static class Util
    {
        public static string ProductVersion = "0.5.4";
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

        public static bool IsMonitorValueLimitError(
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

        public static bool IsMonitorValueLimitWarning(
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

        public static string GetBackgroundCellClass(
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

        public static bool IsAgentUnavailable(Agent agent)
        {
            return (DateTime.UtcNow - agent.LastReplyReceived).TotalMinutes > 10;
        }

        public static string GetColumnTitleLimit(
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



        public static DataTable CreateMonitorDataSet(
                                    List<Agent> agents,
                                    List<MonitorCommand> monitorCommands,
                                    List<MonitorValue> monitorValues,
                                    List<MonitorCommandLimit> monitorCommandLimits)
        {
            var dataSet = new DataSet();

            // Make a empty dataset for the values
            var monitorValueTable = new DataTable("MonitorValue");
            for (var index = 0; index < monitorCommands.Count; index++)
            {
                var dataColumn = new DataColumn(
                                        index.ToString(),
                                        typeof(String));
                monitorValueTable.Columns.Add(dataColumn);
            }
            // create empty row to be filled in later
            foreach (var agent in agents)
            {
                var dataRow = monitorValueTable.NewRow();
                monitorValueTable.Rows.Add(dataRow);
            }
            // Now we need to fill in the empty dataset
            foreach (var monitorValue in monitorValues)
            {
                var agentIndex = agents.FindIndex(
                                            a => a.AgentId == monitorValue.AgentId);
                var monitorCommandIndex = monitorCommands.FindIndex(
                                            a => a.MonitorCommandId == monitorValue.MonitorCommandId);
                monitorValueTable.Rows[agentIndex][monitorCommandIndex] =
                                            $"{monitorValue.Value} {monitorCommands[monitorCommandIndex].Unit}";
            }
            //dataSet.Tables.Add(monitorValueTable);
            return monitorValueTable;
        }
        public static string GenerateHtmlString(List<Agent> agents, int i)
        {
            var city = agents[i].ClientCity != string.Empty ? agents[i].ClientCity + "," : agents[i].ClientCity;
            var str = $@"   
                            <div class='outer'>
                                <div class='outer-div'>
                                    <div class='outer-div-div'>
                                        <div>
                                          {agents[i].ScreenName}
                                        </div>
                                    </div>
                                    <div class='more-info'>                                                                           
                                        {city}
                                       {agents[i].ClientCountry}
                                    </div>
                                </div>
                                <div class='inner-div'> 
                                    <div class='dropdown'>
                                        <div class='dots'
                                             onclick=""myFunction('myDropdown_{agents[i].AgentId}')"">
                                            <div id = ""myDropdown_{agents[i].AgentId}"" class='dropdown-content'>
                                                <a href = 'hardware.aspx?id={agents[i].AgentId}' > Hardware </a>
                                                <a href='software.aspx?id={agents[i].AgentId}'>Software</a>
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
                                                        userNotification.LastNotified).TotalMinutes > 60)
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
            try
            {

                var smtpClient = new SmtpClient(monitorSettings.Email.Server)
                {
                    Port = monitorSettings.Email.Port,
                    Credentials = new NetworkCredential(
                                                monitorSettings.Email.Login,
                                                monitorSettings.Email.Password),
                    EnableSsl = monitorSettings.Email.IsSsl,
                    UseDefaultCredentials = true
                };

                smtpClient.Send(
                            monitorSettings.Email.FromEmail,
                            toEmailAddress,
                            subject,
                            body);
            }
            catch (Exception e)
            {
                throw new Exception($"EmailException:{e.Message}");
            }
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
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    var ip2Component = new IP2Location.Component();
                    ip2Component.IPDatabasePath =
                        System.IO.Path.GetFullPath(
                                            HostingEnvironment.MapPath(
                                                        "~/App_Data/IP2LOCATION-LITE-DB3.BIN"));
                    //"C:\\officeclipnew\\opensource\\devmon_agent\\dev_web_api\\App_Data\\IP2LOCATION-LITE-DB3.BIN";
                    ipResult = ip2Component.IPQuery(ipAddress);
                    switch (ipResult.Status.ToString())
                    {
                        case "OK":
                            cityName = ipResult.City;
                            countryName = ipResult.CountryShort;
                            break;
                        case "EMPTY_IP_ADDRESS":
                            throw new Exception("IP Address cannot be blank.");

                        case "INVALID_IP_ADDRESS":
                            throw new Exception("Invalid IP Address.");

                        case "MISSING_FILE":
                            throw new Exception("Invalid Database Path.");
                    }
                }
                else
                {
                    return "Unknown";
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

        /// <summary>
        /// Retunrn the Full country name like United States of America
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static string GetIpFullInfo(string ipAddress)
        {
            var cityName = string.Empty;
            var countryName = string.Empty;
            IPResult ipResult = new IP2Location.IPResult();
            try
            {
                if (ipAddress != "" && ipAddress != null)
                {
                    var ip2Component = new IP2Location.Component();
                    ip2Component.IPDatabasePath =
                        System.IO.Path.GetFullPath(
                                            HostingEnvironment.MapPath(
                                                        "~/App_Data/IP2LOCATION-LITE-DB3.BIN"));
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
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                ipResult = null;
            }
            return countryName;
        }

    }
}