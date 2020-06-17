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
            grdMonitorHelp.DataSource = GetCommandHelp();
            grdMonitorHelp.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            HiddenField1.Value = "Insert";
            var monitorCommand = new MonitorCommand()
            {
                Name = txtName.Text.Trim(),
                Type = txtType.Text.Trim(),
                Arg1 = txtArg1.Text.Trim(),
                Arg2 = txtArg2.Text.Trim()
            };
            monitorDb.InsertMonitorCommand(monitorCommand);
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
                ((TextBox)GridView1.Rows[e.RowIndex].Cells[position].Controls[0]).Text.Trim();
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
            monitorDb.UpsertCommand(monitorCommand);
            GridView1.EditIndex = -1;
            LoadData();
        }
    }
}