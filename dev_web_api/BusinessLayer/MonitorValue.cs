using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web_api.BusinessLayer
{
    public class MonitorValue
    {
        public int AgentId { get; set; }
        public int MonitorCommandId { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public int ReturnCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}