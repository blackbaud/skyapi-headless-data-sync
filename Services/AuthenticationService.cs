using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Blackbaud.HeadlessDataSync.Services
{
    
    /// <summary>
    /// Contains business logic and helper methods that interact with the authentication provider.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IDataStorageService _dataStorageService;

        /// <summary>
        /// Constructs a new AuthenticationService.
        /// </summary>
        public AuthenticationService(IOptions<AppSettings> appSettings, IDataStorageService dataStorageService)
        {
            _appSettings = appSettings;
            _dataStorageService = dataStorageService;
        }


        /// <summary>
        /// Fetches access/refresh tokens from the provider and sends them to the data 
        /// storage service to be saved for subsequent requests.
        /// </summary>
        /// <param name="requestBody">Key-value attributes to be sent with the request.</param>
        /// <returns>The response from the provider.</returns>
        private async Task<HttpResponseMessage> FetchTokensAsync(Dictionary<string, string> requestBody) 
        {
            using (HttpClient client = new HttpClient()) 
            {   
                // Build token endpoint URL.
                string url = new Uri(new Uri(_appSettings.Value.AuthBaseUri), "token").ToString();

                var clientId = _appSettings.Value.AuthClientId;
                var clientSecret = _appSettings.Value.AuthClientSecret;

                // Set request headers.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization", "Basic " + Base64Encode(_appSettings.Value.AuthClientId + ":" + _appSettings.Value.AuthClientSecret));
                
                // Fetch tokens from auth server.
                HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(requestBody));

                // Save the access/refresh tokens in the Session.
                await _dataStorageService.SetTokensFromResponseAsync(response);
                
                return response;
            }
        }

        /// <summary>
        /// Refreshes the expired access token (from the stored refresh token).
        /// </summary>
        public async Task<HttpResponseMessage> RefreshAccessTokenAsync()
        {
            return await FetchTokensAsync(new Dictionary<string, string>(){
                { "grant_type", "refresh_token" },
                { "refresh_token", _dataStorageService.GetRefreshToken() }
            });
        }
              
        /// <summary>
        /// Encodes a string as Base64.
        /// </summary>
        private static string Base64Encode(string plainText) 
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(bytes);
        }
    }
}
