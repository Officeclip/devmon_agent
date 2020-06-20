using dev_web_api.BusinessLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace dev_web_api.Controllers
{
    public class MonitorCommandsController : ApiController
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        MonitorDb monitorDb = new MonitorDb();
        /// <summary>
        /// GetAll call
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MonitorCommand> GetAllMonitorCommands()
        {
            _logger.Info("-----------------------------------------");
            _logger.Info("Method GetAllMonitorCommands()...");

            // Check to make sure if the org-id and the user-id matches
            // correctly. Also, match the guid to create an register agent 
            // if not present
            var headers = Request.Headers;
            _logger.Info("Request Headers...");
            _logger.Info(headers);
            var serverGuid = headers.GetValues("server_guid").First();
            if (!Util.IsServerGuidValid(serverGuid)){
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            var agent = new Agent()
            {
                Guid = headers.GetValues("agent_guid").First(),
                OrgId = 1, // currently hardcoding but will be read from the header
                MachineName = headers.GetValues("machine_name").First(),
                RegistrationDate = DateTime.UtcNow,
                LastQueried = DateTime.UtcNow
            };
            monitorDb.UpsertAgent(agent);
            var monitorCommands = monitorDb.GetMonitorCommands();
            _logger.Info("Monitor Commands...");
            
            _logger.Info(ObjectDumper.Dump(monitorCommands));
            return monitorCommands;
        }
    }
}
