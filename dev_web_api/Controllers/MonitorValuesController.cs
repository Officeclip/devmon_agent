using dev_web_api.Models;
using NLog;
using System.Collections.Generic;
using System.Web.Http;

namespace dev_web_api.Controllers
{
    public class MonitorValuesController : ApiController
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        [HttpPost]
        public IHttpActionResult Post([FromBody]List<MonitorValue> commandValues)
        {
            _logger.Info("Command Values...");
            _logger.Info(ObjectDumper.Dump(commandValues));
            foreach (var commandValue in commandValues)
            {
                var MonitorValue = new BusinessLayer.MonitorValue()
                {
                    AgentId = 1, // This should be derived from header
                    MonitorCommandId = commandValue.Id,
                    ReturnCode = commandValue.ReturnCode,
                    Value = commandValue.Value,
                    Unit = commandValue.Unit,
                    ErrorMessage = commandValue.ErrorMessage
                };
                (new MonitorDb()).UpdateMonitorValue(MonitorValue);
            }
            return Ok();
        }
    }
}
