using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Quartz
{
    public class ResultInfo
    {
        public string Id { get; set; }
        public bool IsSuccess { get; set; } = true;
        public int ReturnCode { get; set; } = 0;
        public string ErrorMessage { get; set; } = "";
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
