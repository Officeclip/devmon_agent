using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web_api.BusinessLayer
{
    public class AgentGroups
    {
        public int AgentGroupId { get; set; }
        public string AgentGroupName { get; set; }
        public int OrgId { get; set; }
    }
}