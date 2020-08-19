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
      
        protected string chartConfigString;
        public dev_web_api.Graphcontrol graphCtrlHrs;
        protected void Page_Init(object sender, EventArgs e)
        {
         
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }  

    }
}