using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    public interface IRestClientFactory
    {
        IRestClient Create();
    }
}
