using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web_api.BusinessLayer
{
    public class Agent
    {
        public int AgentId { get; set; }
        public string Guid { get; set; }
        public string MachineName { get; set; }
        public int OrgId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}