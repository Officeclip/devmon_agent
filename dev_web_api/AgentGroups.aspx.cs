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
    public partial class AddAgenstoGroup : System.Web.UI.Page
    {
        private Agent agent;
        private int AgentGroupId = -1;
        protected void Page_Load(object sender, EventArgs e)
        {
            //hypnewGrp.NavigateUrl = string.Format("javascript:openPopUp('{0}')", "addagentgroup.aspx");
            if (!IsPostBack)
            {
                var monitorDb = new MonitorDb();
                var agentInfo = monitorDb.GetAgents();
                grdAgents.DataSource = agentInfo;
                grdAgents.DataBind();
            }
            if (Request.QueryString["id"] != string.Empty)
            {
                AgentGroupId = Convert.ToInt32(Request.QueryString["id"]);
            }
        }
        protected void grdAgents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem as Agent;
                Label lbl = (Label)e.Row.FindControl("lblAgentName");
                var hdnAgentId = (HiddenField)e.Row.FindControl("hdnAgentId");
                lbl.Text = item.MachineName;
                hdnAgentId.Value = item.AgentId.ToString();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var monitorDb = new MonitorDb();
            foreach (GridViewRow gvr in grdAgents.Rows)
            {
                var hdnAgentId = Convert.ToInt32(((HiddenField)gvr.FindControl("hdnAgentId")).Value);
                if (((CheckBox)gvr.FindControl("chkAgent")).Checked == true)
                {
                    monitorDb.AddAgentsIntoAgentGroup( AgentGroupId,hdnAgentId);
                }
            }
            Response.Write("<script>window.close();</" + "script>");
            Response.End();
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.close();</" + "script>");
            Response.End();
        }
    }
}