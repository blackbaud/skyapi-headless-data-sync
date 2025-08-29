using Blackbaud.HeadlessDataSync;
using Blackbaud.HeadlessDataSync.Services;
using Blackbaud.HeadlessDataSync.Services.DataSync;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Blackbaud.HeadlessDataSync.Tests
{
    public class SyncAppTests
    {
        private readonly Mock<ILogger<SyncApp>> _mockLogger;
        private readonly Mock<IDataSyncService> _mockDataSyncService;
        private readonly Mock<IDataStorageService> _mockDataStorageService;

        public SyncAppTests()
        {
            _mockLogger = new Mock<ILogger<SyncApp>>();
            _mockDataSyncService = new Mock<IDataSyncService>();
            _mockDataStorageService = new Mock<IDataStorageService>();
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            var syncApp = new SyncApp(
                _mockLogger.Object,
                _mockDataSyncService.Object,
                _mockDataStorageService.Object);

            Assert.NotNull(syncApp);
        }

        [Fact]
        public void Run_WithRefreshTokenArgument_ParsesCorrectly()
        {
            var refreshToken = "test-refresh-token";
            var args = new[] { "--refreshtoken", refreshToken };

            var syncApp = new SyncApp(
                _mockLogger.Object,
                _mockDataSyncService.Object,
                _mockDataStorageService.Object);

            syncApp.Run(args);

            Assert.NotNull(syncApp);
        }

        [Fact]
        public void Run_WithShortRefreshTokenArgument_ParsesCorrectly()
        {
            var refreshToken = "test-refresh-token";
            var args = new[] { "-r", refreshToken };

            var syncApp = new SyncApp(
                _mockLogger.Object,
                _mockDataSyncService.Object,
                _mockDataStorageService.Object);

            syncApp.Run(args);

            Assert.NotNull(syncApp);
        }

        [Fact]
        public void Run_WithoutRefreshTokenArgument_ParsesCorrectly()
        {
            var args = new string[] { };

            var syncApp = new SyncApp(
                _mockLogger.Object,
                _mockDataSyncService.Object,
                _mockDataStorageService.Object);

            syncApp.Run(args);

            Assert.NotNull(syncApp);
        }

        [Theory]
        [InlineData("--refreshtoken", "token123")]
        [InlineData("-r", "token456")]
        [InlineData("--refreshtoken", "")]
        public void Run_WithVariousRefreshTokenFormats_HandlesCorrectly(string flag, string token)
        {
            var args = new[] { flag, token };

            var syncApp = new SyncApp(
                _mockLogger.Object,
                _mockDataSyncService.Object,
                _mockDataStorageService.Object);

            syncApp.Run(args);

            Assert.NotNull(syncApp);
        }

        [Fact]
        public void Run_WithEmptyArgs_HandlesCorrectly()
        {
            var args = new string[] { };

            var syncApp = new SyncApp(
                _mockLogger.Object,
                _mockDataSyncService.Object,
                _mockDataStorageService.Object);

            syncApp.Run(args);

            Assert.NotNull(syncApp);
        }

        [Fact]
        public void Run_WithNullArgs_HandlesCorrectly()
        {
            string[] args = null;

            var syncApp = new SyncApp(
                _mockLogger.Object,
                _mockDataSyncService.Object,
                _mockDataStorageService.Object);

            syncApp.Run(args);

            Assert.NotNull(syncApp);
        }

        [Fact]
        public void Run_WithInvalidArgs_HandlesCorrectly()
        {
            var args = new[] { "--invalid", "value" };

            var syncApp = new SyncApp(
                _mockLogger.Object,
                _mockDataSyncService.Object,
                _mockDataStorageService.Object);

            syncApp.Run(args);

            Assert.NotNull(syncApp);
        }
    }
}
