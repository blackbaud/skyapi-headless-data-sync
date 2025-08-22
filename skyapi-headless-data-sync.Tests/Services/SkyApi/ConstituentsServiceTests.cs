using Blackbaud.HeadlessDataSync.Models;
using Blackbaud.HeadlessDataSync.Services;
using Blackbaud.HeadlessDataSync.Services.SkyApi;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Blackbaud.HeadlessDataSync.Tests.Services.SkyApi
{
    public class ConstituentsServiceTests
    {
        private readonly Mock<IOptions<AppSettings>> _mockAppSettings;
        private readonly Mock<IDataStorageService> _mockDataStorageService;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly AppSettings _appSettings;

        public ConstituentsServiceTests()
        {
            _appSettings = new AppSettings
            {
                SkyApiBaseUri = "https://api.sky.blackbaud.com/",
                SkyApiSubscriptionKey = "test-subscription-key"
            };

            _mockAppSettings = new Mock<IOptions<AppSettings>>();
            _mockAppSettings.Setup(x => x.Value).Returns(_appSettings);

            _mockDataStorageService = new Mock<IDataStorageService>();
            _mockAuthService = new Mock<IAuthenticationService>();
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            var service = new ConstituentsService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object,
                _mockAuthService.Object);

            Assert.NotNull(service);
        }

        [Fact]
        public void GetConstituents_WithValidQueryParams_CallsDataStorageService()
        {
            var queryParams = new ListQueryParams
            {
                LastModified = "2023-01-01T00:00:00Z",
                SortToken = "test-token"
            };

            _mockDataStorageService.Setup(x => x.GetAccessToken()).Returns("valid-access-token");
            _mockAuthService.Setup(x => x.RefreshAccessToken()).Returns(new HttpResponseMessage(HttpStatusCode.OK));

            var service = new ConstituentsService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object,
                _mockAuthService.Object);

            var result = service.GetConstituents(queryParams);

            Assert.NotNull(result);
            _mockDataStorageService.Verify(x => x.GetAccessToken(), Times.AtLeastOnce);
        }

        [Fact]
        public void GetConstituents_WithNullQueryParams_StillMakesRequest()
        {
            _mockDataStorageService.Setup(x => x.GetAccessToken()).Returns("valid-access-token");
            _mockAuthService.Setup(x => x.RefreshAccessToken()).Returns(new HttpResponseMessage(HttpStatusCode.OK));

            var service = new ConstituentsService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object,
                _mockAuthService.Object);

            var result = service.GetConstituents(null);

            Assert.NotNull(result);
            _mockDataStorageService.Verify(x => x.GetAccessToken(), Times.AtLeastOnce);
        }

        [Fact]
        public void CreateQueryParamsFromNextLinkUri_WithValidUri_ExtractsParameters()
        {
            var nextLinkUri = new Uri("https://api.sky.blackbaud.com/constituent/v1/constituents?last_modified=2023-01-01T00:00:00Z&sort_token=abc123");

            var service = new ConstituentsService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object,
                _mockAuthService.Object);

            var result = service.CreateQueryParamsFromNextLinkUri(nextLinkUri);

            Assert.NotNull(result);
            Assert.Equal("2023-01-01T00:00:00Z", result.LastModified);
            Assert.Equal("abc123", result.SortToken);
        }

        [Fact]
        public void CreateQueryParamsFromNextLinkUri_WithUriWithoutParams_ReturnsEmptyParams()
        {
            var nextLinkUri = new Uri("https://api.sky.blackbaud.com/constituent/v1/constituents");

            var service = new ConstituentsService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object,
                _mockAuthService.Object);

            var result = service.CreateQueryParamsFromNextLinkUri(nextLinkUri);

            Assert.NotNull(result);
            Assert.Null(result.LastModified);
            Assert.Null(result.SortToken);
        }

        [Fact]
        public void CreateQueryParamsFromNextLinkUri_WithOnlyLastModified_ExtractsCorrectly()
        {
            var nextLinkUri = new Uri("https://api.sky.blackbaud.com/constituent/v1/constituents?last_modified=2023-01-01T00:00:00Z");

            var service = new ConstituentsService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object,
                _mockAuthService.Object);

            var result = service.CreateQueryParamsFromNextLinkUri(nextLinkUri);

            Assert.NotNull(result);
            Assert.Equal("2023-01-01T00:00:00Z", result.LastModified);
            Assert.Null(result.SortToken);
        }

        [Fact]
        public void CreateQueryParamsFromNextLinkUri_WithOnlySortToken_ExtractsCorrectly()
        {
            var nextLinkUri = new Uri("https://api.sky.blackbaud.com/constituent/v1/constituents?sort_token=abc123");

            var service = new ConstituentsService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object,
                _mockAuthService.Object);

            var result = service.CreateQueryParamsFromNextLinkUri(nextLinkUri);

            Assert.NotNull(result);
            Assert.Null(result.LastModified);
            Assert.Equal("abc123", result.SortToken);
        }

        [Fact]
        public void AppSettings_AreAccessedCorrectly()
        {
            var service = new ConstituentsService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object,
                _mockAuthService.Object);

            _mockAppSettings.Verify(x => x.Value, Times.AtLeastOnce);
        }
    }
}
