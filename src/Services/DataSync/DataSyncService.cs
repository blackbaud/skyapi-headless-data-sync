using Blackbaud.HeadlessDataSync.Services.SkyApi;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Blackbaud.HeadlessDataSync.Services.DataSync
{
    public partial class DataSyncService : IDataSyncService
    {
        private ILogger<DataSyncService> Logger { get; }
        private readonly IDataStorageService _dataStorageService;
        private readonly IConstituentsService _constituentService;

        /// <summary>
        /// Constructs a new DataSyncService.
        /// </summary>
        public DataSyncService(
            ILogger<DataSyncService> logger,
            IDataStorageService dataStorageService,
            IConstituentsService constituentService)
        {
            Logger = logger;
            _dataStorageService = dataStorageService;
            _constituentService = constituentService;
        }

        /// <summary>
        /// Logs an error based on the HTTP status code.
        /// </summary>
        private void ShowError(HttpStatusCode statusCode)
        {
            Logger.LogInformation($"Error: {statusCode}");
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    var errorMessage = "Please ensure your application ID/secret is present in appsettings.json, or you may need to provide a valid refresh token.";
                    Logger.LogInformation(errorMessage);
                    break;
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.NotFound:
                    Logger.LogInformation("Please ensure your SKY API subscription key is present in appsettings.json.");
                    break;
                default:
                    break;
            }
        }
    }
}
