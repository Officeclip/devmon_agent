using dev_web_api.BusinessLayer;
using dev_web_api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class alias : System.Web.UI.Page
    {
        MonitorDb monitorDb = new MonitorDb();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadValues();
            }
        }

        private void LoadValues()
        {
            rptAlias.DataSource = monitorDb.GetAgents();
            rptAlias.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptAlias.Items)
            {
                if (item.ItemType == ListItemType.Item || 
                    item.ItemType == ListItemType.AlternatingItem)
                {
                    var hdnAgentId = (HiddenField)item.FindControl("hdnAgentId");
                    var txtAlias = (TextBox)item.FindControl("txtAlias");
                    var chkEnabled = (CheckBox)item.FindControl("chkEnabled");
                    monitorDb.UpdateAlias(
                        Convert.ToInt32(hdnAgentId.Value),
                        txtAlias.Text.Trim(),
                        chkEnabled.Checked
                        );
                }
            }
            LoadValues();
        }

        protected void rptAlias_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Delete":
                    var agentId = Convert.ToInt32(e.CommandArgument);
                    monitorDb.DeleteAgent(agentId);
                    break;
            }
            LoadValues();
        }

        protected void rptAlias_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || 
                e.Item.ItemType == ListItemType.AlternatingItem)
            {

                LinkButton lnkDelete = e.Item.FindControl("lnkDelete") as LinkButton;
                lnkDelete.Attributes.Add(
                                    "onclick",
                                    "javascript:return confirm('Remove this Agent?')");
            }

        }
    }
}