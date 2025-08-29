using Blackbaud.HeadlessDataSync.Models;
using System;
using Xunit;

namespace Blackbaud.HeadlessDataSync.Tests.Models
{
    public class StorageDataTests
    {
        [Fact]
        public void Properties_CanBeSetAndRetrieved()
        {
            var lastSyncDate = DateTimeOffset.Now;
            var queryParams = new ListQueryParams
            {
                LastModified = "2023-01-01T00:00:00Z",
                SortToken = "test-token"
            };
            var accessToken = "test-access-token";
            var refreshToken = "test-refresh-token";

            var storageData = new StorageData
            {
                LastSyncDate = lastSyncDate,
                ConstituentQueryParams = queryParams,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            Assert.Equal(lastSyncDate, storageData.LastSyncDate);
            Assert.Equal(queryParams, storageData.ConstituentQueryParams);
            Assert.Equal(accessToken, storageData.AccessToken);
            Assert.Equal(refreshToken, storageData.RefreshToken);
        }

        [Fact]
        public void DefaultConstructor_InitializesWithDefaultValues()
        {
            var storageData = new StorageData();

            Assert.Equal(DateTimeOffset.MinValue, storageData.LastSyncDate);
            Assert.Null(storageData.ConstituentQueryParams);
            Assert.Null(storageData.AccessToken);
            Assert.Null(storageData.RefreshToken);
        }

        [Fact]
        public void LastSyncDate_CanBeSetToSpecificDate()
        {
            var specificDate = new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero);
            var storageData = new StorageData
            {
                LastSyncDate = specificDate
            };

            Assert.Equal(specificDate, storageData.LastSyncDate);
        }

        [Fact]
        public void ConstituentQueryParams_CanBeSetToNull()
        {
            var storageData = new StorageData
            {
                ConstituentQueryParams = null
            };

            Assert.Null(storageData.ConstituentQueryParams);
        }

        [Fact]
        public void ConstituentQueryParams_CanBeSetToValidObject()
        {
            var queryParams = new ListQueryParams
            {
                LastModified = "2023-01-01T00:00:00Z",
                SortToken = "test-token"
            };

            var storageData = new StorageData
            {
                ConstituentQueryParams = queryParams
            };

            Assert.NotNull(storageData.ConstituentQueryParams);
            Assert.Equal("2023-01-01T00:00:00Z", storageData.ConstituentQueryParams.LastModified);
            Assert.Equal("test-token", storageData.ConstituentQueryParams.SortToken);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("valid-access-token")]
        public void AccessToken_CanBeSetToVariousValues(string token)
        {
            var storageData = new StorageData
            {
                AccessToken = token
            };

            Assert.Equal(token, storageData.AccessToken);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("valid-refresh-token")]
        public void RefreshToken_CanBeSetToVariousValues(string token)
        {
            var storageData = new StorageData
            {
                RefreshToken = token
            };

            Assert.Equal(token, storageData.RefreshToken);
        }

        [Fact]
        public void AllProperties_CanBeSetIndependently()
        {
            var storageData = new StorageData();

            storageData.LastSyncDate = DateTimeOffset.Now;
            Assert.NotEqual(DateTimeOffset.MinValue, storageData.LastSyncDate);

            storageData.ConstituentQueryParams = new ListQueryParams { LastModified = "test" };
            Assert.NotNull(storageData.ConstituentQueryParams);

            storageData.AccessToken = "access";
            Assert.Equal("access", storageData.AccessToken);

            storageData.RefreshToken = "refresh";
            Assert.Equal("refresh", storageData.RefreshToken);
        }
    }
}
