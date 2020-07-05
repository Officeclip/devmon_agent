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

namespace dev_web_api
{
    public partial class history : System.Web.UI.Page
    {
        MonitorDb monitorDb = new MonitorDb();
        protected void Page_Init(object sender, EventArgs e)
        {
            ddlMonitorCommands.DataSource = monitorDb.GetMonitorCommands();
            ddlMonitorCommands.DataTextField = "Name";
            ddlMonitorCommands.DataValueField = "MonitorCommandId";
            ddlMonitorCommands.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadData();
            }
        }

        private ChartConfiguration CreateServerConfiguration(int monitorCommandId)
        {
            var charts = (new MonitorDb()).GetChart(monitorCommandId);
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
            }
            var xAxesCallback = @"function (value, index, values) {
                                        if (value > 0) { value = -1 * value;}
                                        return value + ' min';
                                    }";
            var yAxesCallback = @"callback: function (value, index, values) {
                                        return value + ' ms';
                                    }";
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
                    Scales =
                    {
                        XAxes =
                        {
                             new Ticks()
                             {
                                 Display = true,
                                 BeginAtZero = true,
                                 Max = 60,
                                 MaxTickLimit = 60,
                                 Callback = new JRaw(xAxesCallback)
                             }
                        },
                        YAxes =
                        {
                             new Ticks()
                             {
                                Callback = new JRaw(yAxesCallback)
                             }
                        }
                    }
                }
            };
            return chartConfig;
        }

        private void LoadData()
        {
            //throw new NotImplementedException();
        }

        [WebMethod]
        public static string GetChart(int monitorCommandId)
        {
            var stringBuilder = new StringBuilder();
            //if (monitorCommandid == -1) return null;
            var charts = (new MonitorDb()).GetChart(monitorCommandId);
            var chart = Util.FixChart(charts[0], 60);
            //if (chart == null) return null;
            stringBuilder.Append("[");
            foreach (var chartPoint in chart.ChartPoints)
            {
            stringBuilder.Append("{");
            stringBuilder.Append($@"""text"": {chartPoint.Minutes}, ""value"": {chartPoint.Value}");
            stringBuilder.Append("},");
            }
            stringBuilder = stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("]");
            return stringBuilder.ToString();
            //return @"[{""text"": ""one"", ""value"": 21}]";
        }

        protected void ddlMonitorCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            var monitorCommandiId = Convert.ToInt32(ddlMonitorCommands.SelectedValue);
        }

    }
}