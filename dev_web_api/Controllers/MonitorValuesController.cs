﻿using dev_web_api.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var serverGuid = headers.GetValues("server_guid").First();
            if (!Util.IsServerGuidValid(serverGuid))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            try
            {
                var guid = headers.GetValues("agent_guid").First();
                var agent = monitorDb.GetAgentByGuid(guid);

                monitorDb.DeleteOldHistory(DateTime.UtcNow);
                foreach (var commandValue in commandValues)
                {
                    var MonitorValue = new BusinessLayer.MonitorValue()
                    {
                        AgentId = agent.AgentId, // This should be derived from header
                        MonitorCommandId = commandValue.Id,
                        ReturnCode = commandValue.ReturnCode,
                        Value = commandValue.Value,
                        ErrorMessage = commandValue.ErrorMessage
                    };
                    _logger.Debug($"MonitorValuesController : UpsertMonitorValue");
                    monitorDb.UpsertMonitorValue(MonitorValue);
                    _logger.Debug($"MonitorValuesController : UpdateLastReceivedReply");
                    monitorDb.UpdateLastReceivedReply(agent.AgentId);
                }
                return Ok();
            }
            catch (Exception e)
            {
                _logger.Error($"Post method Exception:{e.Message}");
                return BadRequest(); 
            }
           
            
        }
    }
}
