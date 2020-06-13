using devmon_library.Core;
using devmon_library.Models;
using Moq;
using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace devmon_test.Core
{
    public class ServerConnectorTests
    {
        readonly ICancellation _mockCancellation;

        public ServerConnectorTests()
        {
            _mockCancellation = new Mock<ICancellation>().Object;
        }

        [Fact]
        public async Task Send_StableDeviceInfo_IsSuccessfull()
        {
            var mockClient = MockRestClient(MockRestResponse);
            var mockSettings = MockSettings();
            var mockSerializer = new Mock<IJsonSerializer>().Object;

            var connector = new ServerConnector(_mockCancellation, mockSettings, mockSerializer, mockClient);

            await connector.Send(new StableDeviceInfo());
        }

        [Fact]
        public async Task Send_VolatileDeviceInfo_IsSuccessfull()
        {
            var mockClient = MockRestClient(MockRestResponse);
            var mockSettings = MockSettings();
            var mockSerializer = new Mock<IJsonSerializer>().Object;

            var connector = new ServerConnector(_mockCancellation, mockSettings, mockSerializer, mockClient);

            await connector.Send(new VolatileDeviceInfo());
        }

        [Fact]
        public void Send_StableDeviceInfo_WithEmptyResponse_ThrowsHttpException()
        {
            var mockClient = MockRestClient((r, c) => new Mock<IRestResponse>().Object);
            var mockSettings = MockSettings();
            var mockSerializer = new Mock<IJsonSerializer>().Object;

            var connector = new ServerConnector(_mockCancellation, mockSettings, mockSerializer, mockClient);

            Assert.ThrowsAsync<HttpException>(() => connector.Send(new StableDeviceInfo()));
        }

        [Fact]
        public void Send_VolatileDeviceInfo_WithEmptyResponse_ThrowsHttpException()
        {
            var mockClient = MockRestClient((r, c) => new Mock<IRestResponse>().Object);
            var mockSettings = MockSettings();
            var mockSerializer = new Mock<IJsonSerializer>().Object;

            var connector = new ServerConnector(_mockCancellation, mockSettings, mockSerializer, mockClient);

            Assert.ThrowsAsync<HttpException>(() => connector.Send(new VolatileDeviceInfo()));
        }

        IRestClientFactory MockRestClient(Func<IRestRequest, CancellationToken, IRestResponse> responseAction)
        {
            var mockClient = new Mock<IRestClient>();
            mockClient
                .Setup(c => c.ExecuteTaskAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseAction);

            var mockClientFactory = new Mock<IRestClientFactory>();
            mockClientFactory.Setup(f => f.Create()).Returns(mockClient.Object);

            return mockClientFactory.Object;
        }

        IAppSettings MockSettings()
        {
            var mockSettings = new Mock<IAppSettings>();
            mockSettings.Setup(s => s[It.IsAny<string>()]).Returns("http://localhost");
            return mockSettings.Object;
        }

        IRestResponse MockRestResponse(IRestRequest request, CancellationToken token)
        {
            var mockResponse = new Mock<IRestResponse>();
            mockResponse.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
            mockResponse.Setup(r => r.IsSuccessful).Returns(true);

            if (string.IsNullOrEmpty(request.Resource))
            {
                mockResponse.Setup(r => r.Content).Returns("{\"access_token\":\"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c\"}");
            }

            return mockResponse.Object;
        }
    }
}
