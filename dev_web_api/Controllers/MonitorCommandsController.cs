﻿using dev_web_api.BusinessLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
namespace dev_web_api.Controllers
{
    public class MonitorCommandsController : ApiController
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";
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
            if (!Util.IsServerGuidValid(serverGuid))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            try
            {
                var agent = new Agent()
                {
                    Guid = headers.GetValues("agent_guid").First(),
                    OrgId = 1, // currently hardcoding but will be read from the header
                    MachineName = headers.GetValues("machine_name").First(),
                    RegistrationDate = DateTime.UtcNow,
                    LastQueried = DateTime.UtcNow,
                    ClientIpAddress = GetClientIpAddress(Request) ?? string.Empty,
                    ProductVersion = headers.GetValues("product_version").First()
            };
                agent.ClientCity = Util.GetIpInfo(agent.ClientIpAddress, false);
                agent.ClientCountry = Util.GetIpInfo(agent.ClientIpAddress, true);
                _logger.Info($"Client Ip address:--{agent.ClientIpAddress}--");
                _logger.Debug($"MonitorCommandsController : UpsertAgent()");
                monitorDb.UpsertAgent(agent);
                var monitorCommands = monitorDb.GetMonitorCommands();
                _logger.Info("Monitor Commands...");
                _logger.Info(ObjectDumper.Dump(monitorCommands));
                return monitorCommands;
            }
            catch (Exception e)
            {
                _logger.Info($"GetMonitorCommands Exception:{e.Message}");
                return null;
            }
        }
        // Reference for this function https://stackoverflow.com/questions/15297620/request-userhostaddress-return-ip-address-of-load-balancer
        public static string GetClientIpAddress(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }
            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }
            return null;
        }
    }
}