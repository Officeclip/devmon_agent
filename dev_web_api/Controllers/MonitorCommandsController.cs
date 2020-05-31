using dev_web_api.BusinessLayer;
using System.Collections.Generic;
using System.Web.Http;

namespace dev_web_api.Controllers
{
    public class MonitorCommandsController : ApiController
    {
        MonitorDb monitorDb = new MonitorDb();
        public IEnumerable<MonitorCommand> GetAllMonitorCommands()
        {
            var monitorCommands = monitorDb.GetMonitorCommands();
            return monitorCommands;
        }
    }
}
