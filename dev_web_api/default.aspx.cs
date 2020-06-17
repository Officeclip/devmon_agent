using dev_web_api.BusinessLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class _default : System.Web.UI.Page
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        List<MonitorCommandLimit> monitorCommandLimits;
        List<Agent> agents;
        List<MonitorCommand> monitorCommands;
        protected string serverGuid = (new MonitorDb()).GetServerGuid(true);
        protected void Page_Load(object sender, EventArgs e)
        {
            MonitorDb monitorDb = new MonitorDb();
            if (monitorDb.GetUsers().Count == 0)
            {
                Response.Redirect("setup.aspx");
            }
            agents = monitorDb.GetAgents();
            monitorCommands = monitorDb.GetMonitorCommands();
            var monitorValues = monitorDb.GetMonitorValues();
            monitorCommandLimits = monitorDb.GetMonitorCommandLimits();
            Util.SetupMonitorTable(
                            ref tblMonitor,
                            agents,
                            monitorCommands,
                            monitorValues,
                            monitorCommandLimits);
            Util.SendMonitorLimitEmail(
                            monitorValues,
                            monitorCommandLimits,
                            monitorCommands);
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

        protected void btnSoftware_Click(object sender, EventArgs e)
        {
            string queryString = "software.aspx";
            string newWin = "window.open('" + queryString + "');";
            ClientScript.RegisterStartupScript(this.GetType(), "software", newWin, true);
        }

        protected void btnAlias_Click(object sender, EventArgs e)
        {
            string queryString = "alias.aspx";
            string newWin = "window.open('" + queryString + "');";
            ClientScript.RegisterStartupScript(this.GetType(), "alias", newWin, true);
        }

        protected void btnSignOff_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("logon.aspx");
        }
    }
}