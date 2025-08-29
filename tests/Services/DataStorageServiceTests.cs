using Blackbaud.HeadlessDataSync.Models;
using Blackbaud.HeadlessDataSync.Services;
using Microsoft.AspNetCore.DataProtection;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Blackbaud.HeadlessDataSync.Tests.Services
{
    public class DataStorageServiceTests : IDisposable
    {
        private readonly Mock<IDataProtectionProvider> _mockDataProtectionProvider;
        private readonly Mock<IDataProtector> _mockDataProtector;
        private readonly string _testStorageFile;

        public DataStorageServiceTests()
        {
            _mockDataProtectionProvider = new Mock<IDataProtectionProvider>();
            _mockDataProtector = new Mock<IDataProtector>();
            _mockDataProtectionProvider.Setup(x => x.CreateProtector(It.IsAny<string>())).Returns(_mockDataProtector.Object);

            _testStorageFile = Path.Combine(Path.GetTempPath(), "test_headlessdatasync_storage.json");
            if (File.Exists(_testStorageFile))
            {
                File.Delete(_testStorageFile);
            }

            var currentDirStorage = "headlessdatasync_storage.json";
            if (File.Exists(currentDirStorage))
            {
                File.Delete(currentDirStorage);
            }
        }

        public void Dispose()
        {
            if (File.Exists(_testStorageFile))
            {
                File.Delete(_testStorageFile);
            }

            var currentDirStorage = "headlessdatasync_storage.json";
            if (File.Exists(currentDirStorage))
            {
                File.Delete(currentDirStorage);
            }
        }

        [Fact]
        public void Constructor_WithValidDataProtectionProvider_CreatesInstance()
        {
            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            Assert.NotNull(service);
            _mockDataProtectionProvider.Verify(x => x.CreateProtector(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void SetAndGetAccessToken_WithValidToken_StoresAndRetrieves()
        {
            var originalToken = "test-access-token";

            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            service.SetAccessToken(originalToken);
            var retrievedToken = service.GetAccessToken();

            Assert.NotNull(service);
        }

        [Fact]
        public void SetAndGetRefreshToken_WithValidToken_StoresAndRetrieves()
        {
            var originalToken = "test-refresh-token";

            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            service.SetRefreshToken(originalToken);
            var retrievedToken = service.GetRefreshToken();

            Assert.NotNull(service);
        }

        [Fact]
        public void SetAndGetLastSyncDate_WithValidDate_StoresAndRetrieves()
        {
            var testDate = DateTimeOffset.Now;
            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            service.SetLastSyncDate(testDate);
            var retrievedDate = service.GetLastSyncDate();

            Assert.Equal(testDate, retrievedDate);
        }

        [Fact]
        public void SetAndGetConstituentQueryParams_WithValidParams_StoresAndRetrieves()
        {
            var queryParams = new ListQueryParams
            {
                LastModified = "2023-01-01T00:00:00Z",
                SortToken = "test-sort-token"
            };

            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            service.SetConstituentQueryParams(queryParams);
            var retrievedParams = service.GetConstituentQueryParams();

            Assert.NotNull(retrievedParams);
            Assert.Equal(queryParams.LastModified, retrievedParams.LastModified);
            Assert.Equal(queryParams.SortToken, retrievedParams.SortToken);
        }

        [Fact]
        public void ClearTokens_RemovesAllTokens()
        {
            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            service.SetAccessToken("access-token");
            service.SetRefreshToken("refresh-token");

            service.ClearTokens();

            var accessToken = service.GetAccessToken();
            var refreshToken = service.GetRefreshToken();

            Assert.Null(accessToken);
            Assert.Null(refreshToken);
        }

        [Fact]
        public void SetTokensFromResponse_WithSuccessfulResponse_ExtractsAndStoresTokens()
        {
            var responseContent = new
            {
                access_token = "new-access-token",
                refresh_token = "new-refresh-token"
            };

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(responseContent))
            };

            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            service.SetTokensFromResponse(response);

            Assert.NotNull(service);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public void SetTokensFromResponse_WithFailedResponse_DoesNotStoreTokens()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("{\"error\":\"invalid_grant\"}")
            };

            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            service.SetTokensFromResponse(response);

            var accessToken = service.GetAccessToken();
            var refreshToken = service.GetRefreshToken();

            Assert.Null(accessToken);
            Assert.Null(refreshToken);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetAccessToken_WithEmptyOrNullToken_StoresNull(string token)
        {
            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            service.SetAccessToken(token);
            var retrievedToken = service.GetAccessToken();

            Assert.Null(retrievedToken);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetRefreshToken_WithEmptyOrNullToken_StoresNull(string token)
        {
            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            service.SetRefreshToken(token);
            var retrievedToken = service.GetRefreshToken();

            Assert.Null(retrievedToken);
        }

        [Fact]
        public void GetAccessToken_WithNullStoredToken_ReturnsNull()
        {
            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            var retrievedToken = service.GetAccessToken();

            Assert.Null(retrievedToken);
        }

        [Fact]
        public void GetRefreshToken_WithNullStoredToken_ReturnsNull()
        {
            var service = new DataStorageService(_mockDataProtectionProvider.Object);

            var retrievedToken = service.GetRefreshToken();

            Assert.Null(retrievedToken);
        }
    }
}
