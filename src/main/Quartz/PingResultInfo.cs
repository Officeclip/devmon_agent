﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Quartz
{
    public class PingResultInfo
    {
        public string Id { get; set; }
        public bool IsSuccess { get; set; }
        public int MilliSeconds { get; set; }
    }
}
