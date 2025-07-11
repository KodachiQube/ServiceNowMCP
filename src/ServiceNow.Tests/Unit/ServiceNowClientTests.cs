using ServiceNow.Services.Clients;
using ServiceNow.Services.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceNow.Tests.Unit
{
    public class ServiceNowClientTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IOptions<ServiceNowConfiguration>> _optionsMock;
        private readonly Mock<ILogger<ServiceNowClient>> _loggerMock;

        public ServiceNowClientTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _optionsMock = new Mock<IOptions<ServiceNowConfiguration>>();
            _loggerMock = new Mock<ILogger<ServiceNowClient>>();
        }

        [Fact]
        public async Task GetAsync_Should_Return_Success_Response()
        {
            // Arrange
            var config = new ServiceNowConfiguration
            {
                InstanceUrl = "https://test.service-now.com",
                Username = "testuser",
                Password = "testpass"
            };
            _optionsMock.Setup(x => x.Value).Returns(config);

            var mockHandler = new MockHttpMessageHandler(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"result\":[]}", Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(mockHandler);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var client = new ServiceNowClient(httpClient, _optionsMock.Object, _loggerMock.Object);

            // Act
            var result = await client.GetAsync("api/now/table/incident");

            // Assert
            result.Should().NotBeNull();
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }
}
