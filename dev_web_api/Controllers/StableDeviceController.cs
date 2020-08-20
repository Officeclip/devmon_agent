using dev_web_api.BusinessLayer;
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
    public class StableDeviceController : ApiController
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        MonitorDb monitorDb = new MonitorDb();
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Post(HttpRequestMessage req)
        {
            _logger.Info("-----------------------------------------");
            _logger.Info("StableDeviceController...");
            var data = req.Content.ReadAsStringAsync().Result;
            data = Regex.Replace(data, @"\s+", " ", RegexOptions.Compiled);
            _logger.Info("StableDevice Results...");
            _logger.Info(data);
            try
            {
                var headers = Request.Headers;
                var serverGuid = headers.GetValues("server_guid").First();
                if (!Util.IsServerGuidValid(serverGuid))
                {
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                }

                var guid = headers.GetValues("agent_guid").First();
                var agent = monitorDb.GetAgentByGuid(guid);
                var agentResource = new AgentResource()
                {
                    AgentId = agent.AgentId,
                    StableDeviceJson = data,
                    LastUpdatedDate = DateTime.UtcNow
                };
                _logger.Debug($"Stable Device controller : UpsertAgentResource");
               
                monitorDb.UpsertAgentResource(agentResource);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.Error($" Stable Device controller :Post method Exception:{e.Message}");
                return BadRequest();
            }

        }
    }
}
