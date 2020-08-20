using dev_web_api.BusinessLayer;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class AddAgensToGroup : System.Web.UI.Page
    {

        private int AgentGroupId = -1;
        protected void Page_Load(object sender, EventArgs e)
        {
            var monitorDb = new MonitorDb();
            if (Request.QueryString["id"] != string.Empty)
            {
                AgentGroupId = Convert.ToInt32(Request.QueryString["id"]);
            }
            if (!IsPostBack)
            {
                var agentInfo = monitorDb.GetAgents();
                grdAgents.DataSource = agentInfo;
                grdAgents.DataBind();
            }


        }
        protected void grdAgents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var monitorDb = new MonitorDb();
            var selectedAgents = monitorDb.GetSelectedAgents(AgentGroupId);
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem as Agent;
                Label lbl = (Label)e.Row.FindControl("lblAgentName");
                CheckBox chkAgent = (CheckBox)e.Row.FindControl("chkAgent");
                var hdnAgentId = (HiddenField)e.Row.FindControl("hdnAgentId");
                lbl.Text = item.MachineName;
                hdnAgentId.Value = item.AgentId.ToString();
                var hdnAgentValue = Convert.ToInt32(hdnAgentId.Value);
                foreach (var agent in selectedAgents)
                {
                    if (hdnAgentValue == agent.AgentId)
                    {
                        chkAgent.Checked = true;
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var monitorDb = new MonitorDb();
            AddAgents(monitorDb);                    
            Response.Write("<script>window.close();</" + "script>");
            Response.End();
        }

        private void AddAgents(MonitorDb monitorDb)
        {
            try
            {
                foreach (GridViewRow gvr in grdAgents.Rows)
                {
                    var hdnAgentId = Convert.ToInt32(((HiddenField)gvr.FindControl("hdnAgentId")).Value);
                    if (((CheckBox)gvr.FindControl("chkAgent")).Checked == true)
                    {
                        monitorDb.AddAgentsIntoAgentGroup(AgentGroupId, hdnAgentId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Method AddAgents: {ex.Message}");
            }
            
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.close();</" + "script>");
            Response.End();
        }
    }
}