using dev_web_api.BusinessLayer;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace dev_web_api.Controllers
{
    public class MonitorCommandsController : ApiController
    {
        MonitorDb monitorDb = new MonitorDb();
        /// <summary>
        /// GetAll call
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MonitorCommand> GetAllMonitorCommands()
        {
            // Check to make sure if the org-id and the user-id matches
            // correctly. Also, match the guid to create an register agent 
            // if not present
            var headers = Request.Headers;
            //var guid = headers.GetValues("agent-guid").First();
            var monitorCommands = monitorDb.GetMonitorCommands();
            return monitorCommands;
        }
    }
}
