using Geheb.DevMon.Agent.Models;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
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
        readonly IRestClient _restClient;
        readonly TokenRequest _tokenRequest;
        readonly Uri _tokenUrl, _serverUrl;
        string _accessToken;

        public ServerConnector(
            ICancellation cancellation,
            IAppSettings settings, 
            IJsonSerializer jsonSerializer, 
            IRestClientFactory restClientFactory)
        {
            _cancellation = cancellation;
            _jsonSerializer = jsonSerializer;

            _restClient = restClientFactory.Create();

            _tokenUrl = new Uri(settings["auth_token_url"] as string);
            _serverUrl = new Uri(settings["server_url"] as string);

            _tokenRequest = new TokenRequest
            {
                ClientId = settings["auth_client_id"] as string,
                ClientSecret = settings["auth_client_secret"] as string,
                Audience = settings["auth_audience"] as string,
                GrantType = "client_credentials"
            };
        }

        public async Task Send(StableDeviceInfo deviceInfo)
        {
            await RequestTokenIfRequired();
            var request = CreateRequest("/stable", deviceInfo, Method.PUT);
            _restClient.BaseUrl = _serverUrl;
            IRestResponse response = await _restClient.ExecuteTaskAsync(request, _cancellation.Token);
            if (!response.IsSuccessful)
            {
                throw new HttpException((int)response.StatusCode, "send stable device info failed: " + 
                    (string.IsNullOrEmpty(response.Content) ? "no response" : response.Content));
            }
        }

        public async Task Send(VolatileDeviceInfo deviceInfo)
        {
            await RequestTokenIfRequired();
            var request = CreateRequest("/volatile", deviceInfo, Method.PUT);
            _restClient.BaseUrl = _serverUrl;
            IRestResponse response = await _restClient.ExecuteTaskAsync(request, _cancellation.Token);
            if (!response.IsSuccessful)
            {
                throw new HttpException((int)response.StatusCode, "send volatile device info failed: " +
                    (string.IsNullOrEmpty(response.Content) ? "no response" : response.Content));
            }
        }

        RestRequest CreateRequest(string resource, object body, Method method = Method.POST)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Authorization", "Bearer " + _accessToken);

            var json = _jsonSerializer.SerializeWithoutFormatting(body);

            request.AddParameter(ContentTypeJson, json, ParameterType.RequestBody);
            return request;
        }

        async Task RequestTokenIfRequired()
        {
            if (!string.IsNullOrEmpty(_accessToken)) return;
            _accessToken = await RequestToken();
        }

        async Task<string> RequestToken()
        {
            var request = CreateRequest(string.Empty, _tokenRequest);
            _restClient.BaseUrl = _tokenUrl;

            var response = await _restClient.ExecuteTaskAsync(request, _cancellation.Token);
            if (!response.IsSuccessful)
            {
                throw new HttpException((int)response.StatusCode, "send auth failed: " + response.Content);
            }

            var obj = JObject.Parse(response.Content);
            var token = (string)obj["access_token"];

            var partToken = token.Split('.');
            if (partToken.Length != 3)
            {
                throw new InvalidDataException("invalid token: " + token);
            }

            var base64Url = new Base64Url();

            var header = JObject.Parse(base64Url.Decode(partToken[0]));
            var data = JObject.Parse(base64Url.Decode(partToken[1]));
            if (!header.HasValues || !data.HasValues)
            {
                throw new FormatException("invalid token: " + token);
            }

            return token;
        }
    }
}
