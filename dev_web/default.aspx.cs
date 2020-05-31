using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var monitorDb = new MonitorDb();
            var agents = monitorDb.GetAgents();
            var monitorCommands = monitorDb.GetMonitorCommands();
            var monitorCommandValues = monitorDb.GetMonitorCommandValues();
            var table = Util.GetMonitorTable(agents, monitorCommands, monitorCommandValues);
            grdMonitor.DataSource = table;
            grdMonitor.DataBind();
        }

        protected void btnPopup_Click(object sender, EventArgs e)
        {
            string queryString = "monitor.aspx";
            string newWin = "window.open('" + queryString + "');";
            ClientScript.RegisterStartupScript(this.GetType(), "pop", newWin, true);
        }
    }
}