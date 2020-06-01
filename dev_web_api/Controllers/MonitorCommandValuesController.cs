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
        [HttpPost]
        public IHttpActionResult Post([FromBody]List<MonitorCommandValue> commandValues)
        {
            //var monitorCommandValues = MonitorCommandValue.FromJson(jsonString);
            return Ok();
        }
    }
}
