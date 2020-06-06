using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class _default : System.Web.UI.Page
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            var monitorDb = new MonitorDb();
            var agents = monitorDb.GetAgents();
            var monitorCommands = monitorDb.GetMonitorCommands();
            var MonitorValues = monitorDb.GetMonitorValues();
            var table = Util.GetMonitorTable(agents, monitorCommands, MonitorValues);
            grdMonitor.DataSource = table;
            grdMonitor.DataBind();
        }

        protected void btnPopup_Click(object sender, EventArgs e)
        {
            string queryString = "monitor.aspx";
            string newWin = "window.open('" + queryString + "');";
            ClientScript.RegisterStartupScript(this.GetType(), "monitor", newWin, true);
        }

        protected void btnHardware_Click(object sender, EventArgs e)
        {
            string queryString = "hardware.aspx";
            string newWin = "window.open('" + queryString + "');";
            ClientScript.RegisterStartupScript(this.GetType(), "hardware", newWin, true);
        }
    }
}