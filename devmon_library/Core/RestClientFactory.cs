using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    internal sealed class RestClientFactory : IRestClientFactory
    {
        public IRestClient Create()
        {
            var restClient = new RestClient();
            restClient.ReadWriteTimeout = 5000;
            restClient.Timeout = 5000;
            return restClient;
        }
    }
}
