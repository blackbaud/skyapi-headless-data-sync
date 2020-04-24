using Blackbaud.HeadlessDataSync.Models;
using System;
using System.Net.Http;

namespace Blackbaud.HeadlessDataSync.Services
{
    public interface IDataStorageService
    {   
        void SetTokensFromResponse(HttpResponseMessage response);
        void ClearTokens();
        DateTimeOffset GetLastSyncDate();
        void SetLastSyncDate(DateTimeOffset lastSyncDate);
        ListQueryParams GetConstituentQueryParams();
        void SetConstituentQueryParams(ListQueryParams queryParams);
        string GetAccessToken();
        string GetRefreshToken();
        void SetRefreshToken(string refreshToken);
    }
}