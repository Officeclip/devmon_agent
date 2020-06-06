using Geheb.DevMon.Agent.Models;
using Geheb.DevMon.Agent.Quartz;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace Geheb.DevMon.Agent.Core
{
    internal sealed class ServerConnector
    {
        const string ContentTypeJson = "application/json";
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ICancellation _cancellation;
        readonly IJsonSerializer _jsonSerializer;
        //readonly IRestClient _iRestClient;
        //readonly TokenRequest _tokenRequest;
        //readonly Uri _tokenUrl;
        readonly Uri _serverUrl;
        string _accessToken;
        readonly RestClient _restClient;
        IAppSettings _settings;

        public ServerConnector(
            ICancellation cancellation,
            IAppSettings settings,
            IJsonSerializer jsonSerializer,
            IRestClientFactory restClientFactory)
        {
            _serverUrl = new Uri(settings["server_url"] as string);
            _restClient = new RestClient(_serverUrl);
            _settings = settings;
            _jsonSerializer = jsonSerializer;
        }

        public async Task AddHeaders(RestRequest request)
        {
            request.AddHeader(
                        _settings["key1"] as string,
                        _settings["value1"] as string);
            request.AddHeader(
                        _settings["key2"] as string,
                        _settings["value2"] as string); 
            request.AddHeader(
                        "agent-guid",
                        _settings["agent_guid"] as string);
            request.AddHeader(
                        "machine-name",
                        Environment.MachineName
                        );
        }

        public async Task Send(StableDeviceInfo deviceInfo)
        {
            var request = CreateRequest("/hardware", deviceInfo, Method.PUT);
            await AddHeaders(request);
            var response = _restClient.Execute(request);
            if (!response.IsSuccessful)
            {
                throw new HttpException(
                                (int)response.StatusCode, 
                                "send stable device info failed: " + 
                                        (string.IsNullOrEmpty(response.Content) 
                                        ? "no response" 
                                        : response.Content));
            }
        }

        public async Task Send(VolatileDeviceInfo deviceInfo)
        {
            var request = CreateRequest("/volatile", deviceInfo, Method.PUT);
            await AddHeaders(request);
            var response = _restClient.Execute(request);
            if (!response.IsSuccessful)
            {
                throw new HttpException(
                                (int)response.StatusCode,
                                "send volatile device info failed: " +
                                        (string.IsNullOrEmpty(response.Content)
                                        ? "no response"
                                        : response.Content));
            }
        }

        public async Task Send(List<ResultInfo> resultInfos)
        {
            var request = CreateRequest("/monitorValues", resultInfos, Method.PUT);
            await AddHeaders(request);
            var response = _restClient.Execute(request);
            if (!response.IsSuccessful)
            {
                throw new HttpException(
                                (int)response.StatusCode,
                                "send volatile device info failed: " +
                                        (string.IsNullOrEmpty(response.Content)
                                        ? "no response"
                                        : response.Content));
            }
        }


        public async Task<IRestResponse> SendPing()
        {
            var request = new RestRequest("/monitorCommands", Method.GET);
            await AddHeaders(request);
            return await _restClient.ExecuteAsync(request);
        }

        RestRequest CreateRequest(
                            string resource, 
                            object body, 
                            Method method = Method.POST)
        {
            var request = new RestRequest(resource, method);
            var json = _jsonSerializer.SerializeWithoutFormatting(body);
            request.AddParameter(ContentTypeJson, json, ParameterType.RequestBody);
            return request;
        }
    }
}
