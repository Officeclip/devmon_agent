using devmon_library.Models;
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

namespace devmon_library.Core
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
                        "server_url",
                        _settings["server_url"] as string);
            request.AddHeader(
                        "server_guid",
                        _settings["server_guid"] as string);
            request.AddHeader(
                        "agent_guid",
                        _settings["agent_guid"] as string);
            request.AddHeader(
                        "machine_name",
                        Environment.MachineName
                        );
            request.AddHeader(
                        "product_version",
                        devmon_library.Util.CurrentVersion
                            );
        }

        public async Task Send(StableDeviceInfo deviceInfo)
        {
            try
            {
                var request = CreateRequest("/stableDevice", deviceInfo, Method.POST);
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
            catch (Exception ex)
            {
                _logger.Error($"Error: {ex.Message}");
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
            _logger.Info("Send Monitor Values...");
            _logger.Info(ObjectDumper.Dump(resultInfos));
            try
            {
                var request = CreateRequest("/monitorValues", resultInfos, Method.POST);
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
            catch (Exception ex)
            {
                _logger.Error($"Error: {ex.Message}");
            }
        }


        public async Task<IRestResponse> SendPing()
        {
            _logger.Info("Get Monitor Commands...");
            IRestResponse restResponse = null;
            try
            {
                var request = new RestRequest("/monitorCommands", Method.GET);
                await AddHeaders(request);
                restResponse = await _restClient.ExecuteAsync(request);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error: {ex.Message}");
                return null;
            }
            return restResponse;
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
