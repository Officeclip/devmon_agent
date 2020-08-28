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
        private readonly MonitorDb _monitorDb = new MonitorDb();
        private List<MonitorCommand> _monitorCommands;
        protected string chartConfigStringForDay;
        //private readonly int _frequency = -1;
        private const int OrgId = 1;

        protected void Page_Init(object sender, EventArgs e)
        {
            _monitorCommands = _monitorDb.GetMonitorCommands();
            ddlMonitorCommands.DataSource = _monitorDb.GetMonitorCommands();
            ddlMonitorCommands.DataTextField = "Name";
            ddlMonitorCommands.DataValueField = "MonitorCommandId";
            ddlMonitorCommands.DataBind();
            LoadAgentGroups();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ddlMonitorCommands.SelectedIndex = ddlFrequancy.SelectedIndex = 0;
                LoadGraph(Convert.ToInt32(
                        ddlMonitorCommands.SelectedValue),
                    Convert.ToInt32(ddlAgentGroups.SelectedValue),
                    (FrequencyTypes)Convert.ToInt32(ddlFrequancy.SelectedValue));
            }
        }

        private void LoadAgentGroups()
        {
            ddlAgentGroups.Items.Clear();
            var info = new AgentGroups()
            {
                AgentGroupId = -1,
                AgentGroupName = "- All -"
            };
            var agentGroupInfo = _monitorDb.GetAgentGroups(OrgId);
            agentGroupInfo.Insert(0, info);
            ddlAgentGroups.DataSource = agentGroupInfo;
            ddlAgentGroups.DataValueField = "AgentGroupId";
            ddlAgentGroups.DataTextField = "AgentGroupName";
            ddlAgentGroups.SelectedValue = "-1";
            ddlAgentGroups.DataBind();
        }

        protected void LoadGraph(
                int monitorCommandId, 
                int agentGrpId, 
                FrequencyTypes selectedFrequency)
        {
            var chartConfig = CreateServerConfiguration(
                monitorCommandId,
                selectedFrequency,
                agentGrpId);
            chartConfigStringForDay = chartConfig?.MakeChart();
        }

        private ChartConfiguration CreateServerConfiguration(
            int monitorCommandId, 
            FrequencyTypes frequency, 
            int agentGroupId)
        {
            var charts = (new MonitorDb()).GetChart(
                monitorCommandId, 
                frequency, 
                agentGroupId);
            //var unitStr = GetUnitString(frequency);
            if ((charts?.Count ?? 0) == 0)
            {
                lblEmptyData.Visible = true;
                return null;
            }

            var dataSets = new List<DataSetItem>();
            try
            {
                for (int i = 0; i < charts.Count; i++)
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

                var unit = _monitorCommands
                    .Find(x => x.MonitorCommandId == monitorCommandId).Unit;
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

                var xAxesTicksItem = new TicksItem() {ticks = xAxesTicks};


                var yAxesCallback = $@"function (value, index, values) {{
                                        return value + ' {unit}';
                                    }}";

                var yAxesTicks = new Ticks()
                {
                    Callback = new JRaw(yAxesCallback)
                };

                var yAxesTicksItem = new TicksItem() {ticks = yAxesTicks};

                var chartConfig = new ChartConfiguration
                {
                    Type = ChartType.line.GetChartType(),
                    Data =
                    {
                        Labels = charts[0]
                            .ReverseChartPointMinutes
                            .ConvertAll<string>(x => x.ToString()),
                        Datasets = dataSets
                    },
                    Options =
                    {
                        //SpanGaps = true,
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
            catch (Exception ex)
            {
                throw new Exception($"Exception:{ex.Message}");
            }
        }

        private string GetxAxesCallback(FrequencyTypes frequency)
        {
            var sb = new StringBuilder("function (value, index, values)");
            sb.Append("{if (value == 0) { return 'Now'; } if (value > 0) {value = -1 * value;}");
            sb.Append($" return value + ' {GetUnitString(frequency)}';");
            sb.Append('}');
            return sb.ToString();
        }

        private String GetUnitString(FrequencyTypes frequency)
        {
            var unitStr = "";
            switch (frequency)
            {
                case FrequencyTypes.Minutes:
                    unitStr = "min";
                    break;
                case FrequencyTypes.Hours:
                    unitStr = "hour";
                    break;
                case FrequencyTypes.Days:
                    unitStr = "day";
                    break;
            }

            return unitStr;
        }

        private int GetMaxUnits(FrequencyTypes frequency)
        {
            int numOfTicks;
            switch (frequency)
            {
                case FrequencyTypes.Minutes:
                    numOfTicks = 60;
                    break;
                case FrequencyTypes.Hours:
                    numOfTicks = 24;
                    break;
                default:
                    numOfTicks = 30;
                    break;
            }
            return numOfTicks;
        }

        protected void ddlMonitorCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGraph(Convert.ToInt32(ddlMonitorCommands.SelectedValue),
                Convert.ToInt32(ddlAgentGroups.SelectedValue),
                (FrequencyTypes)Convert.ToInt32(ddlFrequancy.SelectedValue));
        }

        protected void ddlFrequancy_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGraph(Convert.ToInt32(ddlMonitorCommands.SelectedValue),
                Convert.ToInt32(ddlAgentGroups.SelectedValue),
                (FrequencyTypes)Convert.ToInt32(ddlFrequancy.SelectedValue));
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }

        protected void ddlAgents_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGraph(Convert.ToInt32(ddlMonitorCommands.SelectedValue),
                Convert.ToInt32(ddlAgentGroups.SelectedValue),
                (FrequencyTypes)Convert.ToInt32(ddlFrequancy.SelectedValue));
        }
    }
}