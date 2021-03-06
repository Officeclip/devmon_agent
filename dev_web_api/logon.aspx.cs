﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web_api
{
    public partial class logon : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                MonitorDb monitorDb = new MonitorDb();
                if (monitorDb.GetUsers().Count == 0)
                {
                    Response.Redirect("setup.aspx");
                }
                Msg.Visible = false;
            }
        }

        protected void Submit1_Click(object sender, EventArgs e)
        {
            var users = (new MonitorDb()).GetUsers();
            var user = users.Find(
                                u =>
                                    (u.EmailAddress == UserEmail.Text.Trim()) &&
                                    (u.Password == UserPass.Text.Trim()));
            if (user != null)
            {
                FormsAuthentication.RedirectFromLoginPage(UserEmail.Text, Persist.Checked);
                Response.Redirect("default.aspx");
                Msg.Visible = false;
            }
            else
            {
                Msg.Visible = true;
                Msg.Text = "Invalid credentials. Please try again.";
            }
        }
    }
}