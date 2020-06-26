﻿using dev_web_api.BusinessLayer;
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
    public partial class software : System.Web.UI.Page
    {
        MonitorDb monitorDb = new MonitorDb();
        protected void Page_Init(object sender, EventArgs e)
        {
            ddlAgents.DataSource = monitorDb.GetEnabledAgents();
            ddlAgents.DataValueField = "AgentId";
            ddlAgents.DataTextField = "ScreenName";
            ddlAgents.DataBind();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            var agentResource = monitorDb.GetAgentResource(
                                                Convert.ToInt32(ddlAgents.SelectedValue));
            grdSoftware.Visible = (agentResource != null);
            lblEmptyData.Visible = (agentResource == null);
            if (agentResource != null)
            {
                grdSoftware.DataSource = Softwares(agentResource.StableDeviceJson);
                grdSoftware.DataBind();
            }
        }

        /// <summary>
        /// From: https://stackoverflow.com/a/28492781/89256
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private List<SoftwareInfo> Softwares(string json)
        {
            JToken sJToken = JToken.Parse(json);
            JToken softwaresToken = sJToken["softwares"];
            var softwares = softwaresToken.ToObject<SoftwareInfo[]>().ToList();
            return softwares;
        }

        protected void ddlAgents_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

    }
}