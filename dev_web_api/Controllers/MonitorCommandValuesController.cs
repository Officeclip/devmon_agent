using dev_web_api.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace dev_web_api.Controllers
{
    public class MonitorCommandValuesController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post([FromBody]List<MonitorCommandValue> commandValues)
        {
            foreach (var commandValue in commandValues)
            {
                var monitorCommandValue = new BusinessLayer.MonitorCommandValue()
                {
                    AgentId = 1,
                    MonitorCommandId = commandValue.Id,
                    ReturnCode = commandValue.ReturnCode,
                    Value = commandValue.Value,
                    Unit = commandValue.Unit,
                    ErrorMessage = commandValue.ErrorMessage
                };
                (new MonitorDb()).UpdateMonitorCommandValue(monitorCommandValue);
            }
            return Ok();
        }
    }
}
