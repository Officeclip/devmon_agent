using ChartServerConfiguration.Model;
using dev_web_api.BusinessLayer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class Graphcontrol : System.Web.UI.UserControl
    {
        MonitorDb monitorDb = new MonitorDb();
        List<MonitorCommand> monitorCommands;
        protected string chartConfigStringForDay;
        public int frequency = -1;

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
                ddlMonitorCommands.SelectedIndex = ddlFrequancy.SelectedIndex = 0;
                LoadGraph(Convert.ToInt32(ddlMonitorCommands.SelectedValue), Convert.ToInt32(ddlFrequancy.SelectedValue));
            }
        }

        protected void LoadGraph(int monitorCommandId, int selecedFrequency = 0)
        {
            // var monitorCommandId = Convert.ToInt32(ddlMonitorCommands.SelectedValue);
            var chartConfig = CreateServerConfiguration(monitorCommandId, selecedFrequency);
            chartConfigStringForDay = chartConfig?.MakeChart();
        }

        private void LoadPage()
        {
            var monitorCommandId = Convert.ToInt32(ddlMonitorCommands.SelectedValue);
            var chartConfig = CreateServerConfiguration(monitorCommandId, frequency);
            chartConfigStringForDay = chartConfig?.MakeChart();
        }

        private ChartConfiguration CreateServerConfiguration(int monitorCommandId, int frequency)
        {
            var charts = (new MonitorDb()).GetChart(monitorCommandId, frequency);
            var unitStr = GetUnitString(frequency);
            if (
                (charts == null) ||
                (charts.Count == 0))
            {
                lblEmptyData.Visible = true;
                return null;

            }
            var unit = monitorCommands
                                .Find(x => x.MonitorCommandId == monitorCommandId).Unit;
            var dataSets = new List<DataSetItem>();
            for (int i = 0; i < charts.Count; i++)
            {
                var chart = charts[i];
                var colorCount = LibChart.Util.GetColors().Count;
                var dataSetItem = new DataSetItem()
                {
                    Label = chart.AgentName,
                    Data = chart.ChartPointValues,
                    BorderWidth = 1,
                    BackgroundColor = LibChart.Util.GetColors(i % colorCount),
                    BorderColor = LibChart.Util.GetColors()[i % colorCount],
                    Fill = false
                };
                dataSets.Add(dataSetItem);
            }

            var xAxesCallback = GetxAxesCallback(frequency);
            var units = GetMaxUnits(frequency);
            var xAxesTicks = new Ticks()
            {
                Display = true,
                BeginAtZero = true,
                Max = units,
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

        private string GetxAxesCallback(int frequency)
        {
            var sb = new StringBuilder(@"function (value, index, values)");
            sb.Append ("{ if (value > 0) { value = -1 * value;}") ;
            sb.Append($" return value + ' {GetUnitString(frequency)}';");
            sb.Append('}');
            return sb.ToString();
        }
        private String GetUnitString(int frequency)
        {
            var unitStr = "";
            switch (frequency)
            {
                case 0:
                    unitStr = "min";
                    break;
                case 1:
                    unitStr = "hour";
                    break;
                case 2:
                    unitStr = "day";
                    break;
            }
            return unitStr;
        }

        private int GetMaxUnits(int frequency)
        {
            var numOfTicks = -1;
            if (frequency == 0)
            {
                numOfTicks = 60;
            }
            if (frequency == 1)
            {
                numOfTicks = 24;
            }
            else
            {
                numOfTicks = 30;
            }
            return numOfTicks;
        }

        protected void ddlMonitorCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGraph(Convert.ToInt32(ddlMonitorCommands.SelectedValue), Convert.ToInt32(ddlFrequancy.SelectedValue));
        }

        protected void ddlFrequancy_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGraph(Convert.ToInt32(ddlMonitorCommands.SelectedValue), Convert.ToInt32(ddlFrequancy.SelectedValue));
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }
    }
}