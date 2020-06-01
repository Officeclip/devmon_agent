using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace dev_web_api.Controllers
{
    public class HardwareController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post(HttpRequestMessage req)
        {
            var data = req.Content.ReadAsStringAsync().Result;
            return Ok();
        }
    }
}
