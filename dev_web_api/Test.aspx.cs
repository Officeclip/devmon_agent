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

            var simulatorDb = new simualatorDb();
            simulatorDb.DeleteAllHistory();
            simulatorDb.InsertBulkData(true, 72, 180);

        }
    }


}