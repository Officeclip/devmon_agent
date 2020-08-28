using ChartServerConfiguration.Model;
using dev_web_api.BusinessLayer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class DailyHistory : System.Web.UI.Page
    {
        private readonly MonitorDb _monitorDb = new MonitorDb();
        private List<MonitorCommand> _monitorCommands;
        protected string chartConfigStringForDay;
        protected void Page_Init(object sender, EventArgs e)
        {
            _monitorCommands = _monitorDb.GetMonitorCommands();
            ddlMonitorCommands.DataSource = _monitorDb.GetMonitorCommands();
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
            var chartConfig = CreateServerConfiguration(monitorCommandId, FrequencyTypes.Days);
            chartConfigStringForDay = chartConfig?.MakeChart();
        }

        private ChartConfiguration CreateServerConfiguration(int monitorCommandId, FrequencyTypes frequency)
        {
            var charts = (new MonitorDb()).GetChart(monitorCommandId, frequency, -1);
            if ( (charts?.Count ?? 0) == 0)
            {
                return null;
            }
            var unit = _monitorCommands
                                .Find(x => x.MonitorCommandId == monitorCommandId).Unit;
            var dataSets = new List<DataSetItem>();
            for (var i = 0; i < charts.Count; i++)
            {
                var chart = charts[i];
                var colorCount = LibChart.Util.GetColors().Count;
                var dataSetItem = new DataSetItem()
                {
                    Label = chart.AgentName,
                    Data = chart.ReverseChartPointValues,
                    BorderWidth = 1,
                    BackgroundColor = LibChart.Util.GetColors(i % colorCount),
                    BorderColor = LibChart.Util.GetColors()[i % colorCount],
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
                Max = 24,
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
