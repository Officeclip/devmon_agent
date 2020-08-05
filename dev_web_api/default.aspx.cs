using dev_web_api.BusinessLayer;
using Microsoft.Ajax.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                LoadUserInfo();
                LoadAgentGroups();
                ProcessAndLoadAgents();

            }
        }

        private void ProcessAndLoadAgents()
        {
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
            if (chkEmailOpt.Checked)
            {
                Util.SendMonitorLimitEmail(
                            agents,
                            monitorValues,
                            monitorCommandLimits,
                            monitorCommand);
            }
        }

        private void LoadUserInfo()
        {
            var users = (new MonitorDb()).GetUsers();
            foreach (var uInfo in users)
            {
                if (uInfo.UserId == 1)
                {
                    chkEmailOpt.Checked = uInfo.EmailOptout;
                }
            }
        }

        private void LoadAgentGroups()
        {
            ddlAgentGroups.Items.Clear();
            var info = new AgentGroups()
            {
                AgentGroupId = -1,
                AgentGroupName = "- All -"
            };
            var agentGroupInfo = monitorDb.GetAgentGroups(OrgId);
            agentGroupInfo.Insert(0, info);
            ddlAgentGroups.DataSource = agentGroupInfo;
            ddlAgentGroups.DataValueField = "AgentGroupId";
            ddlAgentGroups.DataTextField = "AgentGroupName";
            ddlAgentGroups.SelectedValue = "-1";
            ddlAgentGroups.DataBind();
        }

        protected string GetWebUri()
        {
            var absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;
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
            ctrl.Attributes["content"] = ConfigurationManager.AppSettings["RefreshFrequency"];
            this.Page.Header.Controls.Add(ctrl);
        }
        private int GetRandomNumber()
        {
            var randomNumber = new Random();
            return randomNumber.Next(100, 500);
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

        protected void chkEmailOpt_CheckedChanged(object sender, EventArgs e)
        {
            var monitorDb = new MonitorDb();
            var optValue = chkEmailOpt.Checked ? 1 : 0;
            // userId is hardcoded here
            monitorDb.InsertEmailOpt(optValue, 1);
            ProcessAndLoadAgents();
        }
    }
}