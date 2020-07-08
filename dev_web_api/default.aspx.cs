using dev_web_api.BusinessLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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
            agents = monitorDb.GetEnabledAgents();
            monitorCommands = monitorDb.GetMonitorCommands();
            var monitorValues = monitorDb.GetMonitorValues();
            monitorCommandLimits = monitorDb.GetMonitorCommandLimits();
            Util.SetupMonitorTable(
                            tblMonitor,
                            agents,
                            monitorCommands,
                            monitorValues,
                            monitorCommandLimits);
            Util.SendMonitorLimitEmail(
                            agents,
                            monitorValues,
                            monitorCommandLimits,
                            monitorCommands);
        }

        protected string GetWebUri()
        {
            var absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;
            //var rawUrl = Request.RawUrl;
            if (absoluteUri.EndsWith("default.aspx"))
            {
                absoluteUri = absoluteUri.Substring(0, absoluteUri.Length - 13);
            }
            return $"{absoluteUri}/api";
        }

        protected void btnPopup_Click(object sender, EventArgs e)
        {
            Response.Redirect("monitor.aspx");
            //string queryString = "monitor.aspx";
            //string newWin = "window.open('" + queryString + "');";
            //ClientScript.RegisterStartupScript(this.GetType(), "monitor", newWin, true);
        }

        protected void btnHardware_Click(object sender, EventArgs e)
        {
            Response.Redirect("hardware.aspx");
            //string queryString = "hardware.aspx";
            //string newWin = "window.open('" + queryString + "');";
            //ClientScript.RegisterStartupScript(this.GetType(), "hardware", newWin, true);
        }

        protected void btnSoftware_Click(object sender, EventArgs e)
        {
            Response.Redirect("software.aspx");
            //string queryString = "software.aspx";
            //string newWin = "window.open('" + queryString + "');";
            //ClientScript.RegisterStartupScript(this.GetType(), "software", newWin, true);
        }

        protected void btnAlias_Click(object sender, EventArgs e)
        {
            Response.Redirect("alias.aspx");
            //string queryString = "alias.aspx";
            //string newWin = "window.open('" + queryString + "');";
            //ClientScript.RegisterStartupScript(this.GetType(), "alias", newWin, true);
        }

        protected void btnSignOff_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("logon.aspx");
        }

        protected void btnCommandLimit_Click(object sender, EventArgs e)
        {
            Response.Redirect("commandLimits.aspx");
            //string queryString = "commandLimits.aspx";
            //string newWin = "window.open('" + queryString + "');";
            //ClientScript.RegisterStartupScript(this.GetType(), "commandLimits", newWin, true);
        }

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            Response.Redirect("history.aspx");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            HtmlGenericControl ctrl = new HtmlGenericControl("meta");
            ctrl.Attributes["http-equiv"] = "refresh";
            ctrl.Attributes["content"] = "30";
            this.Page.Header.Controls.Add(ctrl);
        }
    }
}