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
        public string Alias { get; set; }
        public int OrgId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastQueried { get; set; }
        public DateTime LastReplyReceived { get; set; }
        public bool Enabled { get; set; }
        public string ClientIpAddress { get; set; }

        public string ScreenName
        {
            get
            {
                return
                    !string.IsNullOrEmpty(Alias) && (MachineName != Alias)
                    ? Alias
                    : MachineName;
            }
        }
        
    }
}