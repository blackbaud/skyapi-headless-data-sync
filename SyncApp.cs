using Blackbaud.HeadlessDataSync.Services;
using Blackbaud.HeadlessDataSync.Services.DataSync;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;

namespace Blackbaud.HeadlessDataSync
{
    /// <summary>
    /// Contains app business logic for syncing with Blackbaud data.
    /// </summary>
    public class SyncApp
    {
        private const int SYNC_INTERVAL_MINUTES = 1;

        private ILogger<SyncApp> Logger { get; }
        private readonly IDataSyncService _dataSyncService;
        private readonly IDataStorageService _dataStorageService;

        /// <summary>
        /// Constructs an instance of SyncApp.
        /// </summary>
        public SyncApp(
            ILogger<SyncApp> logger,
            IDataSyncService dataSyncService,
            IDataStorageService dataStorageService)
        {
            Logger = logger;
            _dataSyncService = dataSyncService;
            _dataStorageService = dataStorageService;
        }

        /// <summary>
        /// Runs the app with command line arguments.
        /// </summary>
        public void Run(string[] args)
        {
            var refreshTokenOption = new Option<string>
                (new[] { "-r", "--refreshtoken" }, "The refresh token obtained from initial authorization.");

            var command = new RootCommand
            {
                refreshTokenOption
            };

            command.SetHandler((refreshToken) =>
            {
                SyncData(refreshToken?.ToString());
            },
            refreshTokenOption);

            command.Invoke(args);
        }

        /// <summary>
        /// Syncs data at an interval defined by SYNC_INTERVAL_MINUTES.
        /// </summary>
        /// <param name="refreshToken">The initial token to be used for refreshing access tokens.</param>
        private async void SyncData(string refreshToken)
        {
            Logger.LogInformation("Started data sync");
            if (refreshToken != null)
            {
                Logger.LogInformation($"Setting initial refresh token {refreshToken}");
                _dataStorageService.ClearTokens();
                _dataStorageService.SetRefreshToken(refreshToken);
            }

            // initial sync
            var result = await SyncBBData().ConfigureAwait(false);
            if (result.Contains(false))
            {
                return;
            }

            // Sync every SYNC_INTERVAL_MINUTES
            var timer = new System.Timers.Timer();
            timer.Interval += SYNC_INTERVAL_MINUTES * 60 * 1000;
            timer.Elapsed += async (sender, args) =>
            {
                var result = await SyncBBData().ConfigureAwait(false);
                if (result.Contains(false))
                {
                    // at least one data sync task failed, stop processing
                    timer.Stop();
                }
            };

            timer.Start();
            Console.ReadLine(); // exit when input read from console
        }

        /// <summary>
        /// Syncs with a collection of Blackbaud service data.
        /// </summary>
        /// <returns>An array of boolean indicating if tasks have succeeded.</returns>
        private Task<bool[]> SyncBBData()
        {
            var tasks = new List<Task<bool>>
            {
                _dataSyncService.SyncConstituentDataAsync()
                // additional sync processes...
            };

            return Task.WhenAll(tasks.ToArray());
        }
    }
}
