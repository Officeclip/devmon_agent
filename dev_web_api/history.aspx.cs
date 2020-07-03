using System.Data;
using System.Text;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System;
using System.Web.Script.Services;

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
            var chart = charts[0];
            //if (chart == null) return null;
            stringBuilder.Append("[");
            foreach (var chartPoint in chart.ChartPoints)
            {
            stringBuilder.Append("{");
            stringBuilder.Append($@"""text"": ""{chartPoint.Minutes}"", ""value"": {chartPoint.Value}");
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