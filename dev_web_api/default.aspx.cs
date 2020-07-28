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
        MonitorDb monitorDb = new MonitorDb();
        protected string serverGuid = (new MonitorDb()).GetServerGuid(true);
        protected const int OrgId = 1;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAgentGroups();
                if (ddlAgentGroups.SelectedValue == "-1")
                {
                    agents = monitorDb.GetEnabledAgents();
                }
                else
                {
                    var agentGroupId = Convert.ToInt32(ddlAgentGroups.SelectedValue);
                    if (agentGroupId > 0)
                    {
                        agents = monitorDb.GetAgentsBySelectedGroup(agentGroupId);
                    }
                }
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

        }

        private void LoadAgentGroups()
        {
            ddlAgentGroups.Items.Clear();
            var agentGroupInfo = monitorDb.GetAgentGroups(OrgId);
            var info = new AgentGroups()
            {
                AgentGroupId = -1,
                AgentGroupName = "All groups"
            };
            agentGroupInfo.Add(info);
            ddlAgentGroups.DataSource = agentGroupInfo;
            ddlAgentGroups.DataValueField = "AgentGroupId";
            ddlAgentGroups.DataTextField = "AgentGroupName";
            ddlAgentGroups.SelectedValue = "-1";
            ddlAgentGroups.DataBind();
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
            //var ticks =Convert.ToInt32( DateTime.Now.Ticks);
            var randomNumber = new Random();
            return randomNumber.Next(100, 500);
        }

        protected void btnTestData_Click(object sender, EventArgs e)
        {
            CreateDataForanHour();
            // InsertDataDirectly();
        }
        protected void CreateDataForanHour()
        {
            //var dateTimeNow = DateTime.UtcNow.AddHours(-72);

            //MonitorDb monitorDb = new MonitorDb();
            //var iterationCount = 0;
            //for (var i = 0; i < 48; i++)
            //{
            //    var monitorValue = new MonitorValue
            //    {
            //        AgentId = 1,
            //        MonitorCommandId = 1,
            //        Value = 246,
            //        ErrorMessage = ""
            //    };
            //    var date = dateTimeNow.AddHours(i);
            //    monitorDb.DeleteOldHistory(date);
            //    monitorDb.InsertMonitorHistory(monitorValue, date);
            //    iterationCount = i;

            //}
            //var result = iterationCount;
            InsertDataDirectly();
        }


        protected void InsertDataDirectly()
        {
            var dateTimeNow = DateTime.UtcNow;
            MonitorDb monitorDb = new MonitorDb();
            agents = monitorDb.GetEnabledAgents();
            monitorCommand = monitorDb.GetMonitorCommands();
            foreach (var agent in agents)
            {
                var agentId = agent.AgentId;
                foreach (var command in monitorCommand)
                {
                    var monitorCommand =
                           this.monitorCommand.Find(x => x.MonitorCommandId == command.MonitorCommandId);
                    var monitorValue = new MonitorValue
                    {
                        AgentId = agentId,
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

        protected void ddlAgentGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            var agentGroupId = Convert.ToInt32(ddlAgentGroups.SelectedValue);
            if (agentGroupId > 0)
            {
                agents = monitorDb.GetAgentsBySelectedGroup(agentGroupId);
            }
            else
            {
                agents = monitorDb.GetEnabledAgents();
            }
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
    }
}