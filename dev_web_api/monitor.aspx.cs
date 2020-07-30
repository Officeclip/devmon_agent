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
            }
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
                Type = txtType.Text.Trim(),
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
                ((System.Web.UI.LiteralControl)GridView1.Rows[e.RowIndex].Cells[position].Controls[0]).Text.Trim();
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
                                                            (x => x.Type == txtType.Text.Trim());
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
                Name = GetGridViewText(e, 3),
                Type = GetGridViewText(e, 4),
                Arg1 = GetGridViewText(e, 5),
                Arg2 = GetGridViewText(e, 6)
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
    }
}