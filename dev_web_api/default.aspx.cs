﻿using dev_web_api.BusinessLayer;
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
        List<MonitorCommandLimit> monitorCommandLimits;
        List<Agent> agents;
        List<MonitorCommand> monitorCommands;
        protected void Page_Load(object sender, EventArgs e)
        {
            MonitorDb monitorDb = new MonitorDb();
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