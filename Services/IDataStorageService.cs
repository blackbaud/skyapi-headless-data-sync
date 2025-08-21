using Blackbaud.HeadlessDataSync.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blackbaud.HeadlessDataSync.Services
{
    public interface IDataStorageService
    {   
        void SetTokensFromResponse(HttpResponseMessage response);
        Task SetTokensFromResponseAsync(HttpResponseMessage response);
        void ClearTokens();
        DateTimeOffset GetLastSyncDate();
        void SetLastSyncDate(DateTimeOffset lastSyncDate);
        ListQueryParams GetConstituentQueryParams();
        void SetConstituentQueryParams(ListQueryParams queryParams);
        string GetAccessToken();
        void SetAccessToken(string rawToken);
        string GetRefreshToken();
        void SetRefreshToken(string rawToken);
    }
}
