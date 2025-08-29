using Blackbaud.HeadlessDataSync.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Blackbaud.HeadlessDataSync.Services.DataSync
{
    public partial class DataSyncService
    {
        private DateTime _lastConstituentSyncDate;

        /// <summary>
        /// Syncs with Blackbaud constituent data.
        /// </summary>
        public async Task<bool> SyncConstituentDataAsync()
        {
            _lastConstituentSyncDate = DateTime.Now;
            _dataStorageService.SetLastSyncDate(_lastConstituentSyncDate);

            var queryParams = _dataStorageService.GetConstituentQueryParams();
            if (queryParams == null || queryParams.IsEmpty())
            {
                queryParams = new ListQueryParams
                {
                    LastModified = DateTimeOffset.Now.ToString("o")
                };
            }

            try
            {
                var response = _constituentService.GetConstituents(queryParams);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseData);

                    // Parse and store next_link params
                    Uri.TryCreate(json["next_link"].ToString(), UriKind.Absolute, out var nextLinkUri);
                    _dataStorageService.SetConstituentQueryParams(_constituentService.CreateQueryParamsFromNextLinkUri(nextLinkUri));

                    // Update constituent data
                    UpdateConstituentData(json);

                    return true;
                }

                ShowError(response.StatusCode);

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"Exception: {ex.Message}.");
                if (ex.InnerException is SocketException || ex.InnerException is HttpRequestException)
                {
                    // Exception occurred, continue to retry
                    Logger.LogInformation("Retrying...");
                    return true;
                }

                Logger.LogInformation("Exiting.");
                return false;
            }
        }

        /// <summary>
        /// Update data with modified constituent records.
        /// </summary>
        private void UpdateConstituentData(JObject json)
        {
            // Get count
            var count = int.Parse(json["count"].ToString());
            Logger.LogInformation($"{_lastConstituentSyncDate}: {count} constituents modified since last sync");

            if (count > 0)
            {
                Logger.LogInformation($"Updating {count} records");

                // TODO: Update data with modified constituent records
            }
        }
    }
}
