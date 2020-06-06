using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace dev_web_api.Controllers
{
    public class HardwareController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post(HttpRequestMessage req)
        {
            var data = req.Content.ReadAsStringAsync().Result;
            data = Regex.Replace(data, @"\s+", " ", RegexOptions.Compiled);
            (new MonitorDb()).UpdateAgentResourceHardware(1, data, string.Empty);
            return Ok();
        }
    }
}
