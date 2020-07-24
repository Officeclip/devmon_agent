using dev_web_api.BusinessLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class _default : System.Web.UI.Page
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        List<MonitorCommandLimit> monitorCommandLimits;
        List<Agent> agents;
        List<MonitorCommand> monitorCommand;
        protected string serverGuid = (new MonitorDb()).GetServerGuid(true);
        protected void Page_Load(object sender, EventArgs e)
        {
            MonitorDb monitorDb = new MonitorDb();
            agents = monitorDb.GetEnabledAgents();
            monitorCommand = monitorDb.GetMonitorCommands();
            var monitorValues = monitorDb.GetMonitorValues();
            monitorCommandLimits = monitorDb.GetMonitorCommandLimits();
            Util.SetupMonitorTable(
                            tblMonitor,
                            agents,
                            monitorCommand,
                            monitorValues,
                            monitorCommandLimits);
            Util.SendMonitorLimitEmail(
                            agents,
                            monitorValues,
                            monitorCommandLimits,
                            monitorCommand);
        }

        protected string GetWebUri()
        {
            var absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;
            //var rawUrl = Request.RawUrl;
            if (absoluteUri.EndsWith("default.aspx"))
            {
                absoluteUri = absoluteUri.Substring(0, absoluteUri.Length - 13);
            }
            return $"{absoluteUri}/api";
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            HtmlGenericControl ctrl = new HtmlGenericControl("meta");
            ctrl.Attributes["http-equiv"] = "refresh";
            ctrl.Attributes["content"] = "30";
            this.Page.Header.Controls.Add(ctrl);
        }
        private int GetRandomNumber()
        {
            var randomNumber = new Random();
            return randomNumber.Next(100, 500);
        }

        protected void btnTestData_Click(object sender, EventArgs e)
        {
            /// CreateDataForanHour();
            InsertDataDirectly();
        }
        protected void CreateDataForanHour()
        {
            var dateTimeNow = DateTime.UtcNow;
            var datetime = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, dateTimeNow.Hour, dateTimeNow.Minute, dateTimeNow.Second);
            //  datetime.AddHours(-4);
            MonitorDb monitorDb = new MonitorDb();

            var monitorValue = new MonitorValue
            {
                AgentId = 1,
                MonitorCommandId = 1,
                ErrorMessage = ""
            };
            //for (var i = 0; i < 60; i++)
            //{
            //    monitorValue.Value = GetRandomNumber();
            //    var date = dateTimeNow.AddMinutes(i);
            //    //monitorDb.DeleteOldHistory(date);
            //    //monitorDb.InsertMonitorHistory(monitorValue, date);
            //    //monitorDb.UpdateLastReceivedReply(1);
            //    monitorDb.InsertHistory(monitorValue, date,0);
            //}
            InsertDataDirectly();
        }


        protected void InsertDataDirectly()
        {
            var dateTimeNow = DateTime.UtcNow;
            //var datetime = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, dateTimeNow.Hour, dateTimeNow.Minute, dateTimeNow.Second);
            //datetime.AddHours(-24);
            MonitorDb monitorDb = new MonitorDb();
            var monitorValuesForAgent = new List<MonitorValue>();
            var monitorValues = monitorDb.GetMonitorValues();
            foreach (var agent in monitorValues)
            {
                monitorValuesForAgent = monitorValues
                                            .FindAll(x => x.AgentId == agent.AgentId);
            }
            foreach (var agent in monitorValuesForAgent)
            {

                foreach (var command in monitorCommand)
                {
                    var monitorCommand =
                           this.monitorCommand.Find(x => x.MonitorCommandId == command.MonitorCommandId);
                    var monitorValue = new MonitorValue
                    {
                        AgentId = agent.MonitorCommandId,
                        MonitorCommandId = monitorCommand.MonitorCommandId,
                        ErrorMessage = ""
                    };
                    for (var i = 0; i < 60; i++)
                    {
                        monitorValue.Value = GetRandomNumber();
                        var date = dateTimeNow.AddMinutes(i);
                        monitorDb.InsertHistory(monitorValue, date, 0);
                    }
                    for (var i = 0; i < 24; i++)
                    {
                        monitorValue.Value = GetRandomNumber();
                        var date = dateTimeNow.AddHours(-i);
                        monitorDb.InsertHistory(monitorValue, date, 1);
                    }
                    for (var i = 0; i < 30; i++)
                    {
                        monitorValue.Value = GetRandomNumber();
                        var date = dateTimeNow.AddDays(-i);
                        monitorDb.InsertHistory(monitorValue, date, 2);
                    }
                }
            }
        }

    }
}