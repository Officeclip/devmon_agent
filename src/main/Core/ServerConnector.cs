using Geheb.DevMon.Agent.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using RestSharp.Authenticators;
using System;
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
        }

        public async Task Send(StableDeviceInfo deviceInfo)
        {
            var request = CreateRequest("/stable", deviceInfo, Method.PUT);
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
                                "send stable device info failed: " +
                                        (string.IsNullOrEmpty(response.Content)
                                        ? "no response"
                                        : response.Content));
            }
        }

        public async Task SendPing()
        {
            var request = new RestRequest("/pinger", Method.GET);
            await AddHeaders(request);
            var response = _restClient.Execute(request);
            var body = response.Content;
            //dynamic jsonResponse = JsonConvert.DeserializeObject(body);
            dynamic api = JObject.Parse(body);

            int x;
            x = 2;
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
