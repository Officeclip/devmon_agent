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
                rptAlias.DataSource = monitorDb.GetAgents();
                rptAlias.DataBind();
            }
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
                    monitorDb.UpdateAlias(
                        Convert.ToInt32(hdnAgentId.Value),
                        txtAlias.Text.Trim()
                        );
                }
            }
        }
    }
}