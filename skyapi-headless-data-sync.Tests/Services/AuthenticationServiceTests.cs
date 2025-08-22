using Blackbaud.HeadlessDataSync.Services;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Blackbaud.HeadlessDataSync.Tests.Services
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IOptions<AppSettings>> _mockAppSettings;
        private readonly Mock<IDataStorageService> _mockDataStorageService;
        private readonly AppSettings _appSettings;

        public AuthenticationServiceTests()
        {
            _appSettings = new AppSettings
            {
                AuthBaseUri = "https://oauth2.sky.blackbaud.com/",
                AuthClientId = "test-client-id",
                AuthClientSecret = "test-client-secret"
            };

            _mockAppSettings = new Mock<IOptions<AppSettings>>();
            _mockAppSettings.Setup(x => x.Value).Returns(_appSettings);

            _mockDataStorageService = new Mock<IDataStorageService>();
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            var authService = new AuthenticationService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object);

            Assert.NotNull(authService);
        }

        [Fact]
        public void RefreshAccessToken_WithValidRefreshToken_CallsDataStorageService()
        {
            var refreshToken = "valid-refresh-token";
            
            _mockDataStorageService.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
            _mockDataStorageService.Setup(x => x.SetTokensFromResponse(It.IsAny<HttpResponseMessage>()));

            var authService = new AuthenticationService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object);

            var result = authService.RefreshAccessToken();

            Assert.NotNull(result);
            _mockDataStorageService.Verify(x => x.GetRefreshToken(), Times.Once);
            _mockDataStorageService.Verify(x => x.SetTokensFromResponse(It.IsAny<HttpResponseMessage>()), Times.Once);
        }

        [Fact]
        public void RefreshAccessToken_WithNullRefreshToken_StillMakesRequest()
        {
            _mockDataStorageService.Setup(x => x.GetRefreshToken()).Returns((string)null);
            _mockDataStorageService.Setup(x => x.SetTokensFromResponse(It.IsAny<HttpResponseMessage>()));

            var authService = new AuthenticationService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object);

            var result = authService.RefreshAccessToken();

            Assert.NotNull(result);
            _mockDataStorageService.Verify(x => x.GetRefreshToken(), Times.Once);
            _mockDataStorageService.Verify(x => x.SetTokensFromResponse(It.IsAny<HttpResponseMessage>()), Times.Once);
        }

        [Theory]
        [InlineData("test", "dGVzdA==")]
        [InlineData("client:secret", "Y2xpZW50OnNlY3JldA==")]
        [InlineData("", "")]
        public void Base64Encode_EncodesCorrectly(string input, string expected)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            var result = Convert.ToBase64String(bytes);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void AppSettings_AreAccessedCorrectly()
        {
            var authService = new AuthenticationService(
                _mockAppSettings.Object,
                _mockDataStorageService.Object);

            authService.RefreshAccessToken();

            _mockAppSettings.Verify(x => x.Value, Times.AtLeastOnce);
        }
    }
}
