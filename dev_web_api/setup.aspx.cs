using dev_web_api.BusinessLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class setup : System.Web.UI.Page
    {
        MonitorDb monitorDb = new MonitorDb();
        protected void Page_Init(object sender, EventArgs e)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            monitorDb.InsertUser(
                new BusinessLayer.User()
                {
                    UserId = 1,
                    EmailAddress = txtEmail.Text.Trim(),
                    Password = txtPassword.Text.Trim()
                });
            Response.Redirect("default.aspx");
        }
    }
}