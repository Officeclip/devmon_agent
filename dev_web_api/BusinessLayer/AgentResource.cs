﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web_api.BusinessLayer
{
    public class AgentResource
    {
        public int AgentId { get; set; }
        public string StableDeviceJson { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}