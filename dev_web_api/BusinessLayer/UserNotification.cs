using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web_api.BusinessLayer
{
    public class UserNotification
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public int AgentId { get; set; }
        public int MonitorCommandId { get; set; }
        public DateTime LastNotified { get; set; }
    }
}