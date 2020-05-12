using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Quartz
{
    public class CommandInfo
    {
        public string Id { get; set; }

        public string Command { get; set; }

        [DefaultValue("")]
        public string Arg1 { get; set; }

        [DefaultValue("")]
        public string Arg2 { get; set; }
    }
}
