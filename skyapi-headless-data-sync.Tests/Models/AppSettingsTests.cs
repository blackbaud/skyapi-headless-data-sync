using Blackbaud.HeadlessDataSync.Services;
using Xunit;

namespace Blackbaud.HeadlessDataSync.Tests.Models
{
    public class AppSettingsTests
    {
        [Fact]
        public void Properties_CanBeSetAndRetrieved()
        {
            var authBaseUri = "https://oauth2.sky.blackbaud.com/";
            var authClientId = "test-client-id";
            var authClientSecret = "test-client-secret";
            var skyApiSubscriptionKey = "test-subscription-key";
            var skyApiBaseUri = "https://api.sky.blackbaud.com/";

            var appSettings = new AppSettings
            {
                AuthBaseUri = authBaseUri,
                AuthClientId = authClientId,
                AuthClientSecret = authClientSecret,
                SkyApiSubscriptionKey = skyApiSubscriptionKey,
                SkyApiBaseUri = skyApiBaseUri
            };

            Assert.Equal(authBaseUri, appSettings.AuthBaseUri);
            Assert.Equal(authClientId, appSettings.AuthClientId);
            Assert.Equal(authClientSecret, appSettings.AuthClientSecret);
            Assert.Equal(skyApiSubscriptionKey, appSettings.SkyApiSubscriptionKey);
            Assert.Equal(skyApiBaseUri, appSettings.SkyApiBaseUri);
        }

        [Fact]
        public void DefaultConstructor_InitializesWithNullValues()
        {
            var appSettings = new AppSettings();

            Assert.Null(appSettings.AuthBaseUri);
            Assert.Null(appSettings.AuthClientId);
            Assert.Null(appSettings.AuthClientSecret);
            Assert.Null(appSettings.SkyApiSubscriptionKey);
            Assert.Null(appSettings.SkyApiBaseUri);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("https://oauth2.sky.blackbaud.com/")]
        public void AuthBaseUri_CanBeSetToVariousValues(string value)
        {
            var appSettings = new AppSettings
            {
                AuthBaseUri = value
            };

            Assert.Equal(value, appSettings.AuthBaseUri);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("client-id-123")]
        public void AuthClientId_CanBeSetToVariousValues(string value)
        {
            var appSettings = new AppSettings
            {
                AuthClientId = value
            };

            Assert.Equal(value, appSettings.AuthClientId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("client-secret-456")]
        public void AuthClientSecret_CanBeSetToVariousValues(string value)
        {
            var appSettings = new AppSettings
            {
                AuthClientSecret = value
            };

            Assert.Equal(value, appSettings.AuthClientSecret);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("subscription-key-789")]
        public void SkyApiSubscriptionKey_CanBeSetToVariousValues(string value)
        {
            var appSettings = new AppSettings
            {
                SkyApiSubscriptionKey = value
            };

            Assert.Equal(value, appSettings.SkyApiSubscriptionKey);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("https://api.sky.blackbaud.com/")]
        public void SkyApiBaseUri_CanBeSetToVariousValues(string value)
        {
            var appSettings = new AppSettings
            {
                SkyApiBaseUri = value
            };

            Assert.Equal(value, appSettings.SkyApiBaseUri);
        }

        [Fact]
        public void AllProperties_CanBeSetIndependently()
        {
            var appSettings = new AppSettings();

            appSettings.AuthBaseUri = "auth-base";
            Assert.Equal("auth-base", appSettings.AuthBaseUri);

            appSettings.AuthClientId = "client-id";
            Assert.Equal("client-id", appSettings.AuthClientId);

            appSettings.AuthClientSecret = "client-secret";
            Assert.Equal("client-secret", appSettings.AuthClientSecret);

            appSettings.SkyApiSubscriptionKey = "subscription-key";
            Assert.Equal("subscription-key", appSettings.SkyApiSubscriptionKey);

            appSettings.SkyApiBaseUri = "api-base";
            Assert.Equal("api-base", appSettings.SkyApiBaseUri);
        }
    }
}
