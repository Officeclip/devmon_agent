using dev_web_api.BusinessLayer;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using NLog.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class ProtoTest : System.Web.UI.Page
    {
        private List<MonitorCommandHelp> commandHelp = new List<MonitorCommandHelp>();
        protected void Page_Init(object sender, EventArgs e)
        {
            var monitorCommandHelps = GetCommandHelp();
            LoadTypes(monitorCommandHelps);
        }

        private void LoadTypes(List<MonitorCommandHelp> monitorCommandHelps)
        {
            ddlType.DataSource = monitorCommandHelps;
            ddlType.DataValueField = "Type";
            ddlType.DataTextField = "Type";
            ddlType.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private List<MonitorCommandHelp> GetCommandHelp()
        {
            var json = Util.ReadFile(
                                    Server.MapPath(
                                        @"~/App_Data/monitorCommands.json"));
            commandHelp = JsonConvert.DeserializeObject<List<MonitorCommandHelp>>(json);
            return commandHelp;
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillToolTipInfo(ddlType.SelectedValue);          
        }

        private void FillToolTipInfo(string selectedValue)
        {
            foreach (var command in commandHelp)
            {

                if (command.Type == selectedValue)
                {
                    ValidateCommandArguments(command);
                }
            }

        }

        private void ValidateCommandArguments(MonitorCommandHelp command)
        {
            txtArg1.Enabled = command.Arg1 != string.Empty;
            txtArg2.Enabled = command.Arg2 != string.Empty;
            lblType.Text = command.Description;
            lblArg1.Text = command.Arg1;
            lblArg2.Text = command.Arg2;            
        }
    }
}