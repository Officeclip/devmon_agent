using dev_web_api.BusinessLayer;
using Microsoft.Ajax.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        List<MonitorCommand> monitorCommands;
        MonitorDb monitorDb = new MonitorDb();
        protected string serverGuid = (new MonitorDb()).GetServerGuid(true);
        protected const int OrgId = 1;
        DataSet monitorDataSet;
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
            agents = monitorDb.GetEnabledAgents();
            monitorCommands = monitorDb.GetMonitorCommands();
            var monitorValues = monitorDb.GetMonitorValues();
            monitorCommandLimits = monitorDb.GetMonitorCommandLimits();

            var agentGroupId = Convert.ToInt32(ddlAgentGroups.SelectedValue);

            if (agentGroupId > 0)
            {
                var agentIds = monitorDb.GetAgentIdByAgentGroup(
                                                         agentGroupId);
                var agentEnumerated =
                        from agentResult in agents
                        join agent in agentIds on agentResult.AgentId equals agent
                        select agentResult;

                agents = agentEnumerated.ToList();

                var monitorValueEnumerated =
                    from monitorResult in monitorValues
                    join agent in agentIds on monitorResult.AgentId equals agent
                    select monitorResult;

                monitorValues = monitorValueEnumerated.ToList();
            }

            try
            {
                monitorDataSet = Util.CreateMonitorDataSet(
                                agents,
                                monitorCommands,
                                monitorValues,
                                monitorCommandLimits);
                LoadDataSet();

                Util.SetupMonitorTable(
                                tblMonitor,
                                agents,
                                monitorCommands,
                                monitorValues,
                                monitorCommandLimits);

                if (chkEmailOpt.Checked)
                {
                    Util.SendMonitorLimitEmail(
                                agents,
                                monitorValues,
                                monitorCommandLimits,
                                monitorCommands);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}");
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
            ProcessAndLoadAgents();
         
        }

        protected void chkEmailOpt_CheckedChanged(object sender, EventArgs e)
        {
            var monitorDb = new MonitorDb();
            var optValue = chkEmailOpt.Checked ? 1 : 0;
            // userId is hardcoded here
            monitorDb.InsertEmailOpt(optValue, 1);
            ProcessAndLoadAgents();
        }    

        private void LoadDataSet()
        {
            rptHeader.DataSource = monitorDataSet.Tables["MonitorCommand"];
            rptHeader.DataBind();
            rptRowItem.DataSource = monitorDataSet.Tables["MonitorValue"];
            rptRowItem.DataBind();
        }

        protected string GetColumnSpan()
        {
            return "1";
        }

        protected string GetHeader(object dataIndex)
        {
            var agent = agents[(int)dataIndex];
            return agent.ScreenName;
        }

        protected string GetData(object index, object data)
        {
            var colIndex = Convert.ToInt32(index);
            var strData = data.ToString();
            var commaPosition = strData.IndexOf(",");
            var rowIndex = Convert.ToInt32(strData.Substring(0, commaPosition));
            strData = strData.Substring(commaPosition + 1);
            var isAgentAvailable = !Util.IsAgentUnavailable(agents[rowIndex]);
            if (!isAgentAvailable)
            {
                if (colIndex == 0)
                {
                    var colSpan = monitorCommands.Count;
                    return $@"<td colspan=""{colSpan}"">Agent Not Available</td>";
                }
                else
                {
                    return string.Empty;
                }
            }
            return $"<td>{strData}</td>"; 
        }

        protected void rptRowItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            DataRowView dv = e.Item.DataItem as DataRowView;
            var index = e.Item.ItemIndex;
            if (dv != null) {
                Repeater childRepeater = (Repeater)e.Item.FindControl("rptCellItem");
                if (childRepeater != null) {
                    childRepeater.DataSource = CreateDataSource(index, dv.Row.ItemArray);
                    childRepeater.DataBind();
                }
            }
        }

        private List<string> CreateDataSource(int index, object[] itemArray)
        {
            var dataSource = new List<string>();
            foreach (var item in itemArray)
            {
                dataSource.Add($"{index},{item}");
            }           
            return dataSource;
        }

        protected void rptCellItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }
    }
}