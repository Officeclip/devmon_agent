using dev_web_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace dev_web_api.Controllers
{
    public class MonitorCommandValuesController : ApiController
    {
        public void Post([FromBody]string jsonString)
        {
            var monitorCommandValues = MonitorCommandValue.FromJson(jsonString);
        }
    }
}
