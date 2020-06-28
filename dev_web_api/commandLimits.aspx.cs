using dev_web_api.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class commandLimits : System.Web.UI.Page
    {
        MonitorDb monitorDb = new MonitorDb();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadValues();
            }
        }

        private void LoadValues()
        {
            rptCommandLimits.DataSource = MonitorCommandLimits();
            rptCommandLimits.DataBind();
        }

        private List<MonitorCommandLimit> MonitorCommandLimits()
        {
            var dbLimits = monitorDb.GetMonitorCommandLimits();
            var monitorCommandLimits = new List<MonitorCommandLimit>();

            monitorCommandLimits.Add(
                                    new MonitorCommandLimit()
                                    {
                                        Type = "url.ping",
                                    });
            monitorCommandLimits.Add(
                                    new MonitorCommandLimit()
                                    {
                                        Type = "cpu.percent"
                                    });
            monitorCommandLimits.Add(
                                    new MonitorCommandLimit()
                                    {
                                        Type = "memory.free",
                                        IsLowLimit = true
                                    });
            monitorCommandLimits.Add(
                                    new MonitorCommandLimit()
                                    {
                                        Type = "drive.free",
                                        IsLowLimit = true
                                    });
            foreach (var commandLimit in monitorCommandLimits)
            {
                var dbLimit = dbLimits
                                    .Find(x => x.Type == commandLimit.Type);
                commandLimit.WarningLimit = dbLimit?.WarningLimit;
                commandLimit.ErrorLimit = dbLimit?.ErrorLimit;
            }
            return monitorCommandLimits;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            for (int item = 0; item < rptCommandLimits.Items.Count; item++)
            {
                var monitorLimit = new MonitorCommandLimit();
                var repeaterItem = rptCommandLimits.Items[item];

                var command = repeaterItem.FindControl("lblType") as Label;
                monitorLimit.Type = command.Text;

                var warningLimit = repeaterItem.FindControl("txtWarningLimit") as TextBox;
                monitorLimit.WarningLimit =
                                warningLimit.Text == string.Empty
                                ? (int?)null
                                : Convert.ToInt32(warningLimit.Text.Trim());

                TextBox errorLimit = repeaterItem.FindControl("txtErrorLimit") as TextBox;
                monitorLimit.ErrorLimit =
                                errorLimit.Text == string.Empty
                                ? (int?)null
                                : Convert.ToInt32(errorLimit.Text.Trim());

                var isLowLimit = repeaterItem.FindControl("chkIsLowLimit") as CheckBox;
                monitorLimit.IsLowLimit = isLowLimit.Checked;

                if (
                    (monitorLimit.WarningLimit == null) &&
                    (monitorLimit.ErrorLimit == null))
                {
                    continue;
                }
                monitorDb.UpsertMonitorCommandLimit(monitorLimit);
            }
            LoadValues();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }
    }
}