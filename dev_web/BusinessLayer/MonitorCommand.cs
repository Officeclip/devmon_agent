using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web.BusinessLayer
{
    public class MonitorCommand
    {
        public int MonitorCommandId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
    }
}