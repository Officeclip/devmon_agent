using Geheb.DevMon.Agent.Models;
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
        readonly IRestClient _iRestClient;
        readonly TokenRequest _tokenRequest;
        readonly Uri _tokenUrl, _serverUrl;
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
            //_restClient.Authenticator =
            //                    new SimpleAuthenticator(
            //                            settings["key1"] as string,
            //                            settings["value1"] as string,
            //                            settings["key2"] as string,
            //                            settings["value2"] as string);

            _jsonSerializer = jsonSerializer;

            //_cancellation = cancellation;
            //_iRestClient = restClientFactory.Create();

            //_tokenUrl = new Uri(settings["auth_token_url"] as string);

            //_tokenRequest = new TokenRequest
            //{
            //    ClientId = settings["auth_client_id"] as string,
            //    ClientSecret = settings["auth_client_secret"] as string,
            //    Audience = settings["auth_audience"] as string,
            //    GrantType = "client_credentials"
            //};
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
