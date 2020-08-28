using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using dev_web_api.BusinessLayer;
using FriendlyTime;
using NLog;

namespace dev_web_api
{
    public partial class _default : Page
    {
        protected const int OrgId = 1;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private List<Agent> agents;
        private List<MonitorCommandLimit> monitorCommandLimits;
        private List<MonitorCommand> monitorCommands;
        private DataTable monitorDataTable;
        private readonly MonitorDb monitorDb = new MonitorDb();
        private List<MonitorValue> monitorValues;
        protected string serverGuid = new MonitorDb().GetServerGuid(true);

        protected void Page_Load(object sender, EventArgs e)
        {
            agents = monitorDb.GetEnabledAgents();
            monitorValues = monitorDb.GetMonitorValues();
            monitorCommands = monitorDb.GetMonitorCommands();
            monitorCommandLimits = monitorDb.GetMonitorCommandLimits();
            if (!IsPostBack)
            {
                LoadUserInfo();
                LoadAgentGroups();
                ProcessAndLoadAgents();
            }
        }

        private void ProcessAndLoadAgents()
        {
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
                monitorDataTable = Util.CreateMonitorDataSet(
                    agents,
                    monitorCommands,
                    monitorValues,
                    monitorCommandLimits);
                LoadDataSet();
                if (chkEmailOpt.Checked)
                    Util.SendMonitorLimitEmail(
                        agents,
                        monitorValues,
                        monitorCommandLimits,
                        monitorCommands);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}");
            }
        }

        private void LoadUserInfo()
        {
            var users = new MonitorDb().GetUsers();
            foreach (var uInfo in users)
                if (uInfo.UserId == 1)
                    chkEmailOpt.Checked = uInfo.EmailOptout;
        }

        private void LoadAgentGroups()
        {
            ddlAgentGroups.Items.Clear();
            var info = new AgentGroups
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
            var startUrl = "default.aspx";
            if (absoluteUri.EndsWith(startUrl))
            {
                absoluteUri = absoluteUri.Substring(
                    0, 
                    absoluteUri.Length - startUrl.Length + 1);
            }
            return $"{absoluteUri}/api";
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var ctrl = new HtmlGenericControl("meta");
            ctrl.Attributes["http-equiv"] = "refresh";
            ctrl.Attributes["content"] = ConfigurationManager.AppSettings["RefreshFrequency"];
            Page.Header.Controls.Add(ctrl);
        }


        protected void ddlAgentGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessAndLoadAgents();
        }

        protected void chkEmailOpt_CheckedChanged(object sender, EventArgs e)
        {
           var optValue = chkEmailOpt.Checked ? 1 : 0;
            // userId is hardcoded here
            monitorDb.InsertEmailOpt(optValue, 1);
            ProcessAndLoadAgents();
        }

        private void LoadDataSet()
        {
            rptHeader.DataSource = monitorCommands;
            rptHeader.DataBind();
            rptRowItem.DataSource = monitorDataTable;
            rptRowItem.DataBind();
        }

        protected string GetMonitorCommandWarningLimit(object dataIndex)
        {
            var commands = monitorDb.GetMonitorCommands();
            //var commandLimits = monitorDb.GetMonitorCommandLimits();
            var title = Util.GetColumnTitleLimit(commands[(int) dataIndex], monitorCommandLimits);
            return title;
        }

        protected string GetToolTipInfo(object dataIndex)
        {
            var title = $"Ip: {agents[(int) dataIndex].ClientIpAddress}" + "\n" +
                        $"City: {agents[(int) dataIndex].ClientCity}" + "\n" +
                        $"Country: {Util.GetIpFullInfo(agents[(int) dataIndex].ClientIpAddress)}" + "\n" +
                        $"Agent Version: {agents[(int) dataIndex].ProductVersion}" + "\n" +
                        $"Last Response {agents[(int) dataIndex].LastReplyReceived.ToFriendlyDateTime()} ";
            return title;
        }

        protected string GetHeader(object dataIndex)
        {
            var name = Util.GenerateHtmlString(agents, (int) dataIndex);
            return name;
        }

        protected string GetData(object index, object data)
        {
            var colIndex = Convert.ToInt32(index);
            var strData = data.ToString();
            var commaPosition = strData.IndexOf(",", StringComparison.Ordinal);
            var rowIndex = Convert.ToInt32(strData.Substring(0, commaPosition));
            strData = strData.Substring(commaPosition + 1);
            var isAgentAvailable = !Util.IsAgentUnavailable(agents[rowIndex]);
            var monitorValue = GetMonitorValue(rowIndex, colIndex);
            if (!isAgentAvailable)
            {
                if (colIndex == 0)
                {
                    var colSpan = monitorCommands.Count;
                    return $@"<td colspan=""{colSpan}"" class=""notAvailable"">Agent Not Available</td>";
                }

                return string.Empty;
            }

            if (strData.StartsWith("-2"))
            {
                var title = (monitorValue != null)
                    ? monitorValue.ErrorMessage
                    : string.Empty;
                return $@"<td class=""notAvailable"" title=""{title}""><span>NA</span></td>";
            }

            var limitCssClass = string.Empty;
            var monitorCommandType = monitorCommands[colIndex].Type;
            var monitorCommandLimit = monitorCommandLimits
                .Find(x => x.Type == monitorCommandType);
            if (monitorCommandLimit != null)
            {
                if (Util.IsMonitorValueLimitWarning(monitorValue, monitorCommandLimit))
                {
                    limitCssClass = "class=warningLimit";
                }

                if (Util.IsMonitorValueLimitError(monitorValue, monitorCommandLimit))
                {
                    limitCssClass = "class=errorLimit";
                }
            }

            return $"<td {limitCssClass} >{strData}</td>";
        }

        private MonitorValue GetMonitorValue(int rowIndex, int colIndex)
        {
            var agent = agents[rowIndex];
            var monitorCommand = monitorCommands[colIndex];
            var monitorValue = monitorValues
                .Where(x => x.AgentId == agent.AgentId)
                .Where(x => x.MonitorCommandId == monitorCommand.MonitorCommandId)
                .ToList();
            return monitorValue.Count > 0
                ? monitorValue[0]
                : null;
        }

        protected void rptRowItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var dv = e.Item.DataItem as DataRowView;
            if (dv == null) return;
            var childRepeater = (Repeater) e.Item.FindControl("rptCellItem");
            if (childRepeater == null) return;
            var index = e.Item.ItemIndex;
            childRepeater.DataSource = CreateDataSource(index, dv.Row.ItemArray);
            childRepeater.DataBind();
        }

        private List<string> CreateDataSource(int index, object[] itemArray)
        {
            var dataSource = new List<string>();
            foreach (var item in itemArray) dataSource.Add($"{index},{item}");
            return dataSource;
        }
    }
}