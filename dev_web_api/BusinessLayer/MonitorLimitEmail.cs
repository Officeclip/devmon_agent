using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web_api.BusinessLayer
{
    public class MonitorLimitEmail
    {
        public int UserId { get; set; }
        public int AgentId { get; set; }
        public int MonitorCommandId { get; set; }
        public string ToEmailAddress { get; set; }
        public DateTime LastSent { get; set; }
    }
}