using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web_api.BusinessLayer
{
    public class MonitorCommandLimit
    {
        public string Type { get; set; }
        public int WarningLimit { get; set; }
        public int ErrorLimit { get; set; }
        /// <summary>
        /// If true then limit will be triggered if the value falls below the threshold
        /// </summary>
        public bool IsLowLimit { get; set; }
    }
}