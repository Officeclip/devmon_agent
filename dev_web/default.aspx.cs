using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dev_web
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var sqlite_conn = new SqliteConnection(
                @"Data Source=C:\OfficeClipNew\OpenSource\devmon_agent\monitor.db;Version=3;");
        }
    }
}