using NLog;
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
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        [HttpPost]
        public IHttpActionResult Post(HttpRequestMessage req)
        {
            var data = req.Content.ReadAsStringAsync().Result;
            data = Regex.Replace(data, @"\s+", " ", RegexOptions.Compiled);
            _logger.Info("Hardware Results...");
            _logger.Info(data);
            (new MonitorDb()).UpdateAgentResourceHardware(1, data, string.Empty);
            return Ok();
        }
    }
}
