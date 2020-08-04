using dev_web_api.BusinessLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class monitor : System.Web.UI.Page
    {
        MonitorDb monitorDb = new MonitorDb();
        List<MonitorCommandHelp> monitorCommandHelps;

        protected void Page_Init(object sender, EventArgs e)
        {
            monitorCommandHelps = GetCommandHelp();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadData();
                LoadTypes(monitorCommandHelps);
            }
        }
        private void LoadTypes(List<MonitorCommandHelp> monitorCommandHelps)
        {
            ddlType.DataSource = monitorCommandHelps;
            ddlType.DataValueField = "Type";
            ddlType.DataTextField = "Type";
            ddlType.DataBind();
        }

        private List<MonitorCommandHelp> GetCommandHelp()
        {
            var json = Util.ReadFile(
                                    Server.MapPath(
                                        @"~/App_Data/monitorCommands.json"));
            var commandHelp = JsonConvert.DeserializeObject<List<MonitorCommandHelp>>(json);
            return commandHelp;
        }

        private void LoadData()
        {
            HiddenField1.Value = "View";
            GridView1.DataSource = monitorDb.GetMonitorCommands();
            GridView1.DataBind();
            grdMonitorHelp.DataSource = monitorCommandHelps;
            grdMonitorHelp.DataBind();
        }
        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillToolTipInfo(ddlType.SelectedValue);
        }
        private void FillToolTipInfo(string selectedValue)
        {
            foreach (var command in monitorCommandHelps)
            {

                if (command.Type == selectedValue)
                {
                    ValidateCommandArguments(command);
                }
            }

        }

        private void ValidateCommandArguments(MonitorCommandHelp command)
        {
            txtArg1.Enabled = command.Arg1 != string.Empty;
            txtArg2.Enabled = command.Arg2 != string.Empty;
            lblType.Text = command.Description;
            lblArg1.Text = command.Arg1;
            lblArg2.Text = command.Arg2;
            lblUnit.Text = command.Unit;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (IsError())
            {
                return;
            }
            HiddenField1.Value = "Insert";
            var monitorCommand = new MonitorCommand()
            {
                Name = txtName.Text.Trim(),
                Type = ddlType.SelectedValue.Trim(),
                Arg1 = txtArg1.Text.Trim(),
                Arg2 = txtArg2.Text.Trim()
            };
            monitorCommand.Unit =
                   monitorCommandHelps.Find
                                        (x => x.Type == monitorCommand.Type).Unit;
            monitorDb.InsertMonitorCommand(monitorCommand);
            Response.Redirect(Request.RawUrl);
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            LoadData();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            LoadData();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            HiddenField1.Value = "Delete";
            int id = int.Parse(GridView1.DataKeys[e.RowIndex].Value.ToString());
            monitorDb.DeleteMonitorCommand(id);
            LoadData();
        }

        private string GetGridViewText(GridViewUpdateEventArgs e, int position)
        {
            return
                ((System.Web.UI.WebControls.TextBox)GridView1.Rows[e.RowIndex].Cells[position].Controls[1]).Text.Trim();
        }

        private bool IsError()
        {
            var returnValue = false;
            lblError.Text = "";
            if (txtName.Text.Trim().Length == 0)
            {
                lblError.Text += "<br/>Name is required";
                returnValue = true;
            }

            var monitorCommandHelp = monitorCommandHelps.Find
                                                            (x => x.Type == ddlType.SelectedValue.Trim());
            if (monitorCommandHelp == null)
            {
                lblError.Text += "<br/>Type is incorrect";
                returnValue = true;
            }
            else
            {

                var arg1Length = txtArg1.Text.Trim().Length;
                var arg2Length = txtArg2.Text.Trim().Length;
                if (
                    (arg1Length == 0 && monitorCommandHelp.Arg1.Length > 0) ||
                    (arg1Length > 0 && monitorCommandHelp.Arg1.Length == 0))
                {
                    lblError.Text += "<br/>Arg1 is incorrect";
                    returnValue = true;
                }
                if (
                   (arg2Length == 0 && monitorCommandHelp.Arg2.Length > 0) ||
                   (arg2Length > 0 && monitorCommandHelp.Arg2.Length == 0))
                {
                    lblError.Text += "<br/>Arg2 is incorrect";
                    returnValue = true;
                }
            }
            return returnValue;
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = int.Parse(GridView1.DataKeys[e.RowIndex].Value.ToString());
            HiddenField1.Value = "update";
            var monitorCommand = new MonitorCommand()
            {
                MonitorCommandId = id,
                Name = GetGridViewText(e, 2),
                Type = GetGridViewText(e, 3),
                Arg1 = GetGridViewText(e, 4),
                Arg2 = GetGridViewText(e, 5)
            };
            var monitorCommandHelp =
                   monitorCommandHelps.Find
                                        (x => x.Type == monitorCommand.Type);
            if (monitorCommandHelp == null)
            {
                return;
            }
            monitorCommand.Unit = monitorCommandHelp.Unit;
            monitorDb.UpsertMonitorCommand(monitorCommand);
            GridView1.EditIndex = -1;
            LoadData();
        }



        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowState == DataControlRowState.Edit)
                {
                    var commandHelps = GetCommandHelp();
                    DropDownList dropDownList = e.Row.FindControl("ddlTypes") as DropDownList;
                    dropDownList.DataSource = commandHelps;
                    dropDownList.DataValueField = "Type";
                    dropDownList.DataTextField = "Type";
                    dropDownList.DataBind();
                }
            }
        }
    }
}