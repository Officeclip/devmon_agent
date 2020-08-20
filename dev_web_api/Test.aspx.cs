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
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {

        }



        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void txtData_Click(object sender, EventArgs e)
        {
            var mins = txtMin.Text != string.Empty ? Convert.ToInt32(txtMin.Text) : 0;
            var hrs = txtHrs.Text != string.Empty ? Convert.ToInt32(txtHrs.Text) : 0;
            var days = txtDays.Text != string.Empty ? Convert.ToInt32(txtDays.Text) : 0;

            if (txtMin.Text == string.Empty && txtHrs.Text == string.Empty && txtDays.Text == string.Empty)
            {
                lblError.Visible = true;
                lblError.Text = "Please select the input from above text boxes";
                return;
            }
            else
            {
                var simulatorDb = new simulatorDb();
                simulatorDb.DeleteAllHistory();
                simulatorDb.InsertBulkData
                                        (ckhSimualator.Checked,
                                            mins,
                                           hrs,
                                            days);
            }
        }

        protected void fixHistoryData_Click(object sender, EventArgs e)
        {
            // 1. Find the most current time in the history chart
            // 2. Get timespan by subtracting it from the actual current time
            // 3. Add difference to all the times...
            var db = new simulatorDb();
            db.UpdateHistoryWithCurrentTime();
            db.UpdatAgentsDate();
        }
    }


}