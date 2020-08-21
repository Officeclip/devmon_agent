﻿using dev_web_api.BusinessLayer;
using FriendlyTime;
using Microsoft.Ajax.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
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
        List<MonitorValue> monitorValues;
        MonitorDb monitorDb = new MonitorDb();
        protected string serverGuid = (new MonitorDb()).GetServerGuid(true);
        protected const int OrgId = 1;
        DataTable monitorDataTable;
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
            rptHeader.DataSource = monitorCommands;
            rptHeader.DataBind();
            rptRowItem.DataSource = monitorDataTable;
            rptRowItem.DataBind();
        }

        protected string GetMonitorCommandWarningLimit(object dataIndex)
        {
            var commands = monitorDb.GetMonitorCommands();
            var commandLimits = monitorDb.GetMonitorCommandLimits();
            var title = Util.GetColumnTitleLimit(commands[(int)dataIndex], monitorCommandLimits);
            return title;
        }

        protected string GetToolTipInfo(object dataIndex)
        {

            //var agent = agents[(int)dataIndex];
            var title = $"Ip: {agents[(int)dataIndex].ClientIpAddress}" + "\n" +
                    $"City: {agents[(int)dataIndex].ClientCity}" + "\n" +
                    $"Country: {Util.GetIpFullInfo(agents[(int)dataIndex].ClientIpAddress)}" + "\n" +
                    $"Agent Version: {agents[(int)dataIndex].ProductVersion}" + "\n" +
                    $"Last Response {agents[(int)dataIndex].LastReplyReceived.ToFriendlyDateTime()} ";
            return title;

        }
        protected string GetHeader(object dataIndex)
        {
            var name = Util.GenerateHtmlString(agents, (int)dataIndex);
            return name;
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
                    return $@"<td colspan=""{colSpan}"" class=""notAvailable"">Agent Not Available</td>";
                }
                else
                {
                    return string.Empty;
                }
            }
            var rawStr = data.ToString().Replace(",", "");
            var splitValue = Convert.ToDouble(Regex.Split(rawStr, @"[^0-9\.]+").Where(c => c != "." && c.Trim() != "").ToList()[0]);
            var cssValue = GetCssClassForTd(splitValue);
            return $"<td class='{cssValue}'>{strData}</td>"; ;
        }

        public string GetCssClassForTd(double value)
        {
            var cssClass = string.Empty;
            var values = monitorDb.GetMonitorValues();
            var monitorValue = values.Find(x => x.Value == value);
            if (monitorValue != null)
            {
                cssClass = Util.GetBackgroundCellClass(
                       monitorValue,
                       monitorCommands,
                       monitorCommandLimits);
            }
            return cssClass;
        }



        public string SetTabelCell(double value, string strData)
        {
            var values = monitorDb.GetMonitorValues();
            var monitorValue = values.Find(x => x.Value == value);
            var tdStr = "";
            if (monitorValue != null)
            {
                //switch (monitorValue.ReturnCode)
                //{
                //    //case -2:
                //    //    tdStr = $@"<td title =""{monitorValue.ErrorMessage}"" class=""notAvailable"">
                //    //                <span style=""border-bottom: 1px dashed black"">NA</span>
                //    //                </td>";
                //    //    break;
                //    //case -1:
                //    //    tdStr = $@"<td title =""{monitorValue.ErrorMessage}"" class=""notAvailable"">
                //    //                <span style=""border-bottom: 1px dashed black"">Error</span>
                //    //                </td>";
                //    //    break;
                //    default:                                       
                //        break;
                //}
                var monitorCommand = monitorCommands
                                                      .Find(x => x.MonitorCommandId == monitorValue.MonitorCommandId);
                var cssClass = Util.GetBackgroundCellClass(
                        monitorValue,
                        monitorCommands,
                        monitorCommandLimits);
                tdStr = $"<td class='{cssClass}'>{strData}</td>";
            }
            return tdStr;
        }

        protected void rptRowItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            DataRowView dv = e.Item.DataItem as DataRowView;
            var index = e.Item.ItemIndex;
            if (dv != null)
            {
                Repeater childRepeater = (Repeater)e.Item.FindControl("rptCellItem");
                if (childRepeater != null)
                {
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