using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmon_library.Quartz
{
    public class CommandInfo
    {
        public string MonitorCommandId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        [DefaultValue("")]
        public string Arg1 { get; set; }

        [DefaultValue("")]
        public string Arg2 { get; set; }
    }
}
