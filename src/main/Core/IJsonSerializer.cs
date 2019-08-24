using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geheb.DevMon.Agent.Core
{
    public interface IJsonSerializer
    {
        string SerializeWithFormatting(object value);
        string SerializeWithoutFormatting(object value);
    }
}
