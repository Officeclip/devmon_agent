﻿using dev_web_api.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Validation;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class agentGroup : System.Web.UI.Page
    {
        private MonitorDb monitorDb = new MonitorDb();
        private const int orgId = 1;
        public bool isGroupSaved = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
            lblmsg.Visible = false;
        }
        private void LoadData()
        {

            HiddenField1.Value = "View";
            var agentGroupInfo = monitorDb.GetAgentGroups(orgId);
            grdGroups.DataSource = agentGroupInfo;
            grdGroups.DataBind();
            grdGroups.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            if (txtName.Text != string.Empty && !isGroupSaved)
            {
                monitorDb.InsertAgentGroup(txtName.Text);
                txtName.Text = "";
                isGroupSaved = true;
            }
            else
            {
                lblmsg.Visible = true;
                lblmsg.Text = "Input can not be empty!";
            }
            LoadData();
        }


        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }

        protected void grdGroups_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grdGroups.EditIndex = e.NewEditIndex;
            LoadData();
        }

        protected void grdGroups_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grdGroups.EditIndex = -1;
            LoadData();
        }

        protected void grdGroups_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            HiddenField1.Value = "Delete";
            int id = int.Parse(grdGroups.DataKeys[e.RowIndex].Value.ToString());
            monitorDb.DeleteAgentGroup(id);
            LoadData();
        }

        protected void grdGroups_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = int.Parse(grdGroups.DataKeys[e.RowIndex].Value.ToString());
            HiddenField1.Value = "update";
            var agentGroup = new AgentGroups()
            {
                AgentGroupId = id,
                AgentGroupName = GetGridViewText(e, 2)
            };
            monitorDb.UpdateAgentGroup(agentGroup);
            grdGroups.EditIndex = -1;
            LoadData();
        }
        private string GetGridViewText(GridViewUpdateEventArgs e, int position)
        {
            return
                ((TextBox)grdGroups.Rows[e.RowIndex].Cells[position].Controls[1]).Text.Trim();
        }

        protected void grdGroups_DataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem as AgentGroups;
                var lnkAgents = (HyperLink)e.Row.FindControl("lnkAgents");
                lnkAgents.NavigateUrl = GetAgentScreenWindow(item.AgentGroupId);
            }
        }
       // txtGrps

        private string GetAgentScreenWindow(int agentGroupId)
        {
            return
                     $"javascript:openPopUp('AddAgentsToGroups.aspx?id={agentGroupId}')";
        }
    }
}