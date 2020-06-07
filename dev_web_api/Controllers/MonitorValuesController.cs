using dev_web_api.Models;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace dev_web_api.Controllers
{
    public class MonitorValuesController : ApiController
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        MonitorDb monitorDb = new MonitorDb();
        [HttpPost]
        public IHttpActionResult Post([FromBody]List<MonitorValue> commandValues)
        {
            _logger.Info("-----------------------------------------");
            _logger.Info("MonitorValuesController...");
            _logger.Info(ObjectDumper.Dump(commandValues));
            var headers = Request.Headers;
            var guid = headers.GetValues("agent-guid").First();
            var agent = monitorDb.GetAgentByGuid(guid);

            foreach (var commandValue in commandValues)
            {
                var MonitorValue = new BusinessLayer.MonitorValue()
                {
                    AgentId = agent.AgentId, // This should be derived from header
                    MonitorCommandId = commandValue.Id,
                    ReturnCode = commandValue.ReturnCode,
                    Value = commandValue.Value,
                    Unit = commandValue.Unit,
                    ErrorMessage = commandValue.ErrorMessage
                };
                monitorDb.UpsertMonitorValue(MonitorValue);
                monitorDb.UpdateLastReceivedReply(agent.AgentId);
            }
            return Ok();
        }
    }
}
