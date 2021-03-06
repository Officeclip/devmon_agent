﻿using dev_web_api.BusinessLayer;
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

        protected string GetText(object value)
        {
            var islimitExceeded = Convert.ToBoolean(value);

            return islimitExceeded == true ? "Yes" : "No";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            UpsertMonitorMonitorCommandLimits();
        }

        private void UpsertMonitorMonitorCommandLimits()
        {
            try
            {

                for (int item = 0; item < rptCommandLimits.Items.Count; item++)
                {
                    var monitorLimit = new MonitorCommandLimit();
                    var repeaterItem = rptCommandLimits.Items[item];

                    var command = repeaterItem.FindControl("lblType") as Label;
                    if (command == null) return;
                    monitorLimit.Type = command.Text;

                    var warningLimit = repeaterItem.FindControl("txtWarningLimit") as TextBox;
                    if (warningLimit == null) return;
                    monitorLimit.WarningLimit =
                                    warningLimit.Text == string.Empty
                                    ? (int?)null
                                    : Convert.ToInt32(warningLimit.Text.Trim());

                    TextBox errorLimit = repeaterItem.FindControl("txtErrorLimit") as TextBox;
                    if (errorLimit == null) return;
                    monitorLimit.ErrorLimit =
                                    errorLimit.Text == string.Empty
                                    ? (int?)null
                                    : Convert.ToInt32(errorLimit.Text.Trim());
                    
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
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}");
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }
    }
}