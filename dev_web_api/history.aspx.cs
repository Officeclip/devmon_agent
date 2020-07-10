using System.Data;
using System.Text;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System;
using System.Web.Script.Services;
using ChartServerConfiguration.Model;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using dev_web_api.BusinessLayer;

namespace dev_web_api
{
    public partial class history : System.Web.UI.Page
    {
        MonitorDb monitorDb = new MonitorDb();
        List<MonitorCommand> monitorCommands; 
        protected string chartConfigString;
        protected void Page_Init(object sender, EventArgs e)
        {
            monitorCommands = monitorDb.GetMonitorCommands();
            ddlMonitorCommands.DataSource = monitorDb.GetMonitorCommands();
            ddlMonitorCommands.DataTextField = "Name";
            ddlMonitorCommands.DataValueField = "MonitorCommandId";
            ddlMonitorCommands.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            var monitorCommandId = Convert.ToInt32(ddlMonitorCommands.SelectedValue);
            var chartConfig = CreateServerConfiguration(monitorCommandId);
            chartConfigString = chartConfig?.MakeChart();
        }

        private ChartConfiguration CreateServerConfiguration(int monitorCommandId)
        {
            var charts = (new MonitorDb()).GetChart(monitorCommandId);
            if (
                (charts == null) ||
                (charts.Count == 0))
            {
                return null;
            }
            var unit = monitorCommands
                                .Find(x => x.MonitorCommandId == monitorCommandId).Unit;
            var dataSets = new List<DataSetItem>();
            for (int i = 0; i < charts.Count; i++)
            {
                var chart = charts[i];
                var dataSetItem = new DataSetItem()
                {
                    Label = chart.AgentName,
                    Data = chart.ChartPointValues,
                    BorderWidth = 1,
                    BackgroundColor = Util.GetColors(),
                    BorderColor = Util.GetColors()[i % 6],
                    Fill = false
                };
                dataSets.Add(dataSetItem);
            }

            var xAxesCallback = @"function (value, index, values) {
                                        if (value > 0) { value = -1 * value;}
                                        return value + ' min';
                                    }";

            var xAxesTicks = new Ticks()
            {
                Display = true,
                BeginAtZero = true,
                Max = 60,
                MaxTicksLimit = 12,
                Callback = (new JRaw(xAxesCallback))
            };

            var xAxesTicksItem = new TicksItem() { ticks = xAxesTicks };


            var yAxesCallback = $@"function (value, index, values) {{
                                        return value + ' {unit}';
                                    }}";

            var yAxesTicks = new Ticks()
            {
                Callback = new JRaw(yAxesCallback)
            };

            var yAxesTicksItem = new TicksItem() { ticks = yAxesTicks };

            var chartConfig = new ChartConfiguration
            {
                Type = ChartType.line.GetChartType(),
                Data =
                {
                    Labels = charts[0]
                                    .ChartPointMinutes
                                    .ConvertAll<string>(x => x.ToString()),
                    Datasets = dataSets
                },
                Options =
                {
                    Title =
                    {
                        Text = ddlMonitorCommands.SelectedItem.Text
                    },
                    Scales = new Scales()
                    {
                        XAxes = new List<TicksItem>()
                        {
                            xAxesTicksItem
                        },
                        YAxes = new List<TicksItem>()
                        {
                            yAxesTicksItem
                        }
                    }
                }
            };
            return chartConfig;
        }

        protected void ddlMonitorCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPage();
        }

    }
}