using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Blackbaud.HeadlessDataSync.Models;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;

namespace Blackbaud.HeadlessDataSync.Services
{
    
    /// <summary>
    /// Persists data and keys for subsequent app sessions.
    /// </summary>
    public class DataStorageService : IDataStorageService
    {
        private const string STORAGE_DATA_FILE_NAME = "headlessdatasync_storage.json";
        private const string TOKEN_PROTECTOR_NAME = "HeadlessDataSync.Storage.Tokens.v1";

        private readonly IDataProtector _dataProtector;

        private StorageData _storageData;

        /// <summary>
        /// Constructs a new DataStorageService and attempts to read data from storage.
        /// </summary>
        public DataStorageService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector = dataProtectionProvider.CreateProtector(TOKEN_PROTECTOR_NAME);
            ReadStorageData();
        }
        
        
        /// <summary>
        /// Removes access and refresh tokens from storage.
        /// </summary>
        public void ClearTokens()
        {
            try 
            {
                SetAccessToken(string.Empty);
                SetRefreshToken(string.Empty);
            }
            catch (Exception error)
            {
                Console.WriteLine("LOGOUT ERROR: " + error.Message);
            }
        }

        /// <summary>
        /// Gets the date that the last sync occurred.
        /// </summary>
        public DateTimeOffset GetLastSyncDate()
        {
            return _storageData.LastSyncDate;
        }

        /// <summary>
        /// Sets the date that the last sync occurred.
        /// </summary>
        public void SetLastSyncDate(DateTimeOffset lastSyncDate)
        {
            _storageData.LastSyncDate = lastSyncDate;
            WriteStorageData();
        }

        /// <summary>
        /// Gets the cached constituent query params to use when fetching constituent records.
        /// </summary>
        public ListQueryParams GetConstituentQueryParams()
        {
            return _storageData.ConstituentQueryParams;
        }

        /// <summary>
        /// Sets the cached constituent query params to use when fetching subsequent constituent records.
        /// </summary>
        public void SetConstituentQueryParams(ListQueryParams queryParams)
        {
            _storageData.ConstituentQueryParams = queryParams;
            WriteStorageData();
        }

        /// <summary>
        /// Decrypts and returns access token, if saved, otherwise null.
        /// </summary>
        public string GetAccessToken()
        {
            return _storageData.AccessToken != null ?
                _dataProtector.Unprotect(_storageData.AccessToken) :
                null;
        }

        /// <summary>
        /// Encrypts and stores the access token to be used for subsequent requests.
        /// </summary>
        public void SetAccessToken(string rawToken)
        {
            _storageData.AccessToken = !string.IsNullOrEmpty(rawToken) ?
                _dataProtector.Protect(rawToken) :
                null;
            WriteStorageData();
        }

        /// <summary>
        /// Decrypts and returns refresh token, if saved, otherwise null.
        /// </summary>
        public string GetRefreshToken()
        {
            return _storageData.RefreshToken != null ?
                _dataProtector.Unprotect(_storageData.RefreshToken) :
                null;
        }

        /// <summary>
        /// Encrypts and stores the refresh token to be used for subsequent requests.
        /// </summary>
        public void SetRefreshToken(string rawToken)
        {
            _storageData.RefreshToken = !string.IsNullOrEmpty(rawToken) ? 
                _dataProtector.Protect(rawToken) :
                null;
            WriteStorageData();
        }

        /// <summary>
        /// Sets the access and refresh tokens based on an HTTP response.
        /// </summary>
        public void SetTokensFromResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string jsonString = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, string> attrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                SetAccessToken(attrs["access_token"]);
                SetRefreshToken(attrs["refresh_token"]);
            }
        }

        private void ReadStorageData()
        {
            using (var fileStream = File.Open(STORAGE_DATA_FILE_NAME, FileMode.OpenOrCreate, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                _storageData = JsonConvert.DeserializeObject<StorageData>(reader.ReadToEnd());

                if (_storageData == null)
                {
                    _storageData = new StorageData();
                }
            }
        }
        
        private void WriteStorageData()
        {
            using (var fileStream = File.Open(STORAGE_DATA_FILE_NAME, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(JsonConvert.SerializeObject(_storageData, Formatting.Indented));
            }
        }
    }
}
