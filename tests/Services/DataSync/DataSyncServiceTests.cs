using Blackbaud.HeadlessDataSync.Models;
using Blackbaud.HeadlessDataSync.Services;
using Blackbaud.HeadlessDataSync.Services.DataSync;
using Blackbaud.HeadlessDataSync.Services.SkyApi;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace Blackbaud.HeadlessDataSync.Tests.Services.DataSync
{
    public class DataSyncServiceTests
    {
        private readonly Mock<ILogger<DataSyncService>> _mockLogger;
        private readonly Mock<IDataStorageService> _mockDataStorageService;
        private readonly Mock<IConstituentsService> _mockConstituentsService;

        public DataSyncServiceTests()
        {
            _mockLogger = new Mock<ILogger<DataSyncService>>();
            _mockDataStorageService = new Mock<IDataStorageService>();
            _mockConstituentsService = new Mock<IConstituentsService>();
        }

        [Fact]
        public async Task SyncConstituentDataAsync_WithSuccessfulResponse_ReturnsTrue()
        {
            var queryParams = new ListQueryParams
            {
                LastModified = "2023-01-01T00:00:00Z",
                SortToken = "test-token"
            };

            var responseContent = new JObject
            {
                ["count"] = 5,
                ["value"] = new JArray(),
                ["next_link"] = "https://api.sky.blackbaud.com/constituent/v1/constituents?sort_token=next"
            };

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent.ToString())
            };

            var nextLinkParams = new ListQueryParams { SortToken = "next" };

            _mockDataStorageService.Setup(x => x.GetConstituentQueryParams()).Returns(queryParams);
            _mockConstituentsService.Setup(x => x.GetConstituents(queryParams)).Returns(response);
            _mockConstituentsService.Setup(x => x.CreateQueryParamsFromNextLinkUri(It.IsAny<Uri>())).Returns(nextLinkParams);

            var service = new DataSyncService(
                _mockLogger.Object,
                _mockDataStorageService.Object,
                _mockConstituentsService.Object);

            var result = await service.SyncConstituentDataAsync();

            Assert.True(result);
            _mockDataStorageService.Verify(x => x.SetLastSyncDate(It.IsAny<DateTimeOffset>()), Times.Once);
            _mockDataStorageService.Verify(x => x.SetConstituentQueryParams(nextLinkParams), Times.Once);
        }

        [Fact]
        public async Task SyncConstituentDataAsync_WithNullQueryParams_CreatesNewParams()
        {
            var responseContent = new JObject
            {
                ["count"] = 0,
                ["value"] = new JArray(),
                ["next_link"] = "https://api.sky.blackbaud.com/constituent/v1/constituents?sort_token=next"
            };

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent.ToString())
            };

            _mockDataStorageService.Setup(x => x.GetConstituentQueryParams()).Returns((ListQueryParams)null);
            _mockConstituentsService.Setup(x => x.GetConstituents(It.IsAny<ListQueryParams>())).Returns(response);
            _mockConstituentsService.Setup(x => x.CreateQueryParamsFromNextLinkUri(It.IsAny<Uri>())).Returns(new ListQueryParams());

            var service = new DataSyncService(
                _mockLogger.Object,
                _mockDataStorageService.Object,
                _mockConstituentsService.Object);

            var result = await service.SyncConstituentDataAsync();

            Assert.True(result);
            _mockConstituentsService.Verify(x => x.GetConstituents(It.Is<ListQueryParams>(p => !string.IsNullOrEmpty(p.LastModified))), Times.Once);
        }

        [Fact]
        public async Task SyncConstituentDataAsync_WithEmptyQueryParams_CreatesNewParams()
        {
            var emptyParams = new ListQueryParams();
            var responseContent = new JObject
            {
                ["count"] = 0,
                ["value"] = new JArray(),
                ["next_link"] = "https://api.sky.blackbaud.com/constituent/v1/constituents"
            };

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent.ToString())
            };

            _mockDataStorageService.Setup(x => x.GetConstituentQueryParams()).Returns(emptyParams);
            _mockConstituentsService.Setup(x => x.GetConstituents(It.IsAny<ListQueryParams>())).Returns(response);
            _mockConstituentsService.Setup(x => x.CreateQueryParamsFromNextLinkUri(It.IsAny<Uri>())).Returns(new ListQueryParams());

            var service = new DataSyncService(
                _mockLogger.Object,
                _mockDataStorageService.Object,
                _mockConstituentsService.Object);

            var result = await service.SyncConstituentDataAsync();

            Assert.True(result);
            _mockConstituentsService.Verify(x => x.GetConstituents(It.Is<ListQueryParams>(p => !string.IsNullOrEmpty(p.LastModified))), Times.Once);
        }

        [Fact]
        public async Task SyncConstituentDataAsync_WithUnsuccessfulResponse_ReturnsFalse()
        {
            var queryParams = new ListQueryParams { LastModified = "2023-01-01T00:00:00Z" };
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            _mockDataStorageService.Setup(x => x.GetConstituentQueryParams()).Returns(queryParams);
            _mockConstituentsService.Setup(x => x.GetConstituents(queryParams)).Returns(response);

            var service = new DataSyncService(
                _mockLogger.Object,
                _mockDataStorageService.Object,
                _mockConstituentsService.Object);

            var result = await service.SyncConstituentDataAsync();

            Assert.False(result);
        }

        [Fact]
        public async Task SyncConstituentDataAsync_WithSocketException_ReturnsTrue()
        {
            var queryParams = new ListQueryParams { LastModified = "2023-01-01T00:00:00Z" };
            var socketException = new SocketException();
            var httpException = new HttpRequestException("Network error", socketException);

            _mockDataStorageService.Setup(x => x.GetConstituentQueryParams()).Returns(queryParams);
            _mockConstituentsService.Setup(x => x.GetConstituents(queryParams)).Throws(httpException);

            var service = new DataSyncService(
                _mockLogger.Object,
                _mockDataStorageService.Object,
                _mockConstituentsService.Object);

            var result = await service.SyncConstituentDataAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task SyncConstituentDataAsync_WithHttpRequestException_ReturnsTrue()
        {
            var queryParams = new ListQueryParams { LastModified = "2023-01-01T00:00:00Z" };
            var innerHttpException = new HttpRequestException("Network timeout");
            var outerException = new Exception("Outer exception", innerHttpException);

            _mockDataStorageService.Setup(x => x.GetConstituentQueryParams()).Returns(queryParams);
            _mockConstituentsService.Setup(x => x.GetConstituents(queryParams)).Throws(outerException);

            var service = new DataSyncService(
                _mockLogger.Object,
                _mockDataStorageService.Object,
                _mockConstituentsService.Object);

            var result = await service.SyncConstituentDataAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task SyncConstituentDataAsync_WithOtherException_ReturnsFalse()
        {
            var queryParams = new ListQueryParams { LastModified = "2023-01-01T00:00:00Z" };
            var exception = new InvalidOperationException("Unexpected error");

            _mockDataStorageService.Setup(x => x.GetConstituentQueryParams()).Returns(queryParams);
            _mockConstituentsService.Setup(x => x.GetConstituents(queryParams)).Throws(exception);

            var service = new DataSyncService(
                _mockLogger.Object,
                _mockDataStorageService.Object,
                _mockConstituentsService.Object);

            var result = await service.SyncConstituentDataAsync();

            Assert.False(result);
        }

        [Fact]
        public async Task SyncConstituentDataAsync_WithMultipleConstituents_LogsCorrectCount()
        {
            var queryParams = new ListQueryParams { LastModified = "2023-01-01T00:00:00Z" };
            var responseContent = new JObject
            {
                ["count"] = 10,
                ["value"] = new JArray(),
                ["next_link"] = "https://api.sky.blackbaud.com/constituent/v1/constituents?sort_token=next"
            };

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent.ToString())
            };

            _mockDataStorageService.Setup(x => x.GetConstituentQueryParams()).Returns(queryParams);
            _mockConstituentsService.Setup(x => x.GetConstituents(queryParams)).Returns(response);
            _mockConstituentsService.Setup(x => x.CreateQueryParamsFromNextLinkUri(It.IsAny<Uri>())).Returns(new ListQueryParams());

            var service = new DataSyncService(
                _mockLogger.Object,
                _mockDataStorageService.Object,
                _mockConstituentsService.Object);

            var result = await service.SyncConstituentDataAsync();

            Assert.True(result);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("10 constituents modified")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SyncConstituentDataAsync_WithZeroConstituents_DoesNotLogUpdate()
        {
            var queryParams = new ListQueryParams { LastModified = "2023-01-01T00:00:00Z" };
            var responseContent = new JObject
            {
                ["count"] = 0,
                ["value"] = new JArray(),
                ["next_link"] = "https://api.sky.blackbaud.com/constituent/v1/constituents?sort_token=next"
            };

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent.ToString())
            };

            _mockDataStorageService.Setup(x => x.GetConstituentQueryParams()).Returns(queryParams);
            _mockConstituentsService.Setup(x => x.GetConstituents(queryParams)).Returns(response);
            _mockConstituentsService.Setup(x => x.CreateQueryParamsFromNextLinkUri(It.IsAny<Uri>())).Returns(new ListQueryParams());

            var service = new DataSyncService(
                _mockLogger.Object,
                _mockDataStorageService.Object,
                _mockConstituentsService.Object);

            var result = await service.SyncConstituentDataAsync();

            Assert.True(result);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Updating")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }
    }
}
