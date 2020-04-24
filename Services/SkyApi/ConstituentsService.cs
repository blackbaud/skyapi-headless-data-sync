using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using Blackbaud.HeadlessDataSync.Models;
using Microsoft.Extensions.Options;

namespace Blackbaud.HeadlessDataSync.Services.SkyApi
{

    /// <summary>
    /// Interacts directly with SKY API Constituent endpoints.
    /// </summary>
    public class ConstituentsService : IConstituentsService
    {

        private readonly Uri _apiBaseUri;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IDataStorageService _dataStorageService;
        private readonly IAuthenticationService _authService;

        public ConstituentsService(
            IOptions<AppSettings> appSettings,
            IDataStorageService dataStorageService,
            IAuthenticationService authService)
        {
            _appSettings = appSettings;
            _dataStorageService = dataStorageService;
            _authService = authService;
            _apiBaseUri = new Uri(new Uri(_appSettings.Value.SkyApiBaseUri), "constituent/v1/");
        }

        /// <summary>
        /// Requests the auth service to refresh the access token and returns true if successful.
        /// </summary>
        private bool TryRefreshToken()
        {
            HttpResponseMessage tokenResponse = _authService.RefreshAccessToken();
            return (tokenResponse.IsSuccessStatusCode);
        }


        /// <summary>
        /// Performs HTTP requests (POST/GET) and returns the response.
        /// </summary>
        /// <param name="method" type="String">The HTTP method, post, get</param>
        /// <param name="endpoint" type="String">The API endpoint</param>
        /// <param name="content" type="HttpContent">The request body content</param>
        private HttpResponseMessage Proxy(string method, string endpoint, StringContent content = null)
        {
            using (HttpClient client = new HttpClient())
            {
                string token = _dataStorageService.GetAccessToken();
                HttpResponseMessage response;

                // Set constituent endpoint.
                client.BaseAddress = _apiBaseUri;

                // Set request headers.
                client.DefaultRequestHeaders.Add("bb-api-subscription-key", _appSettings.Value.SkyApiSubscriptionKey);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);

                // Make the request to constituent API.
                switch (method.ToLower())
                {
                    default:
                    case "get":
                        response = client.GetAsync(endpoint).Result;
                        break;

                    case "post":
                        response = client.PostAsync(endpoint, content).Result;
                        break;
                }

                return response;
            }
        }

        /// <summary>
        /// Returns a response containing the added/modified constituents using the provided query params.
        /// </summary>
        /// <param name="queryParams">The query parameters to be sent with the request.</param>
        public HttpResponseMessage GetConstituents(ListQueryParams queryParams)
        {
            var query = BuildQueryString(queryParams);
            HttpResponseMessage response = Proxy("get", $"constituents?{query}");

            // Handle bad response.
            if (!response.IsSuccessStatusCode)
            {
                int statusCode = (int)response.StatusCode;
                switch (statusCode)
                {
                    // Request formatted incorrectly.
                    case 400:
                        response.Content = new StringContent("{ error: \"The specified constituents request was not in the correct format.\" }");
                        break;

                    // Token expired/invalid. Refresh the token and try again.
                    case 401:
                        bool tokenRefreshed = TryRefreshToken();
                        if (tokenRefreshed)
                        {
                            response = Proxy("get", $"constituents?{query}");
                        }
                        break;

                    // Constituents not found.
                    case 404:
                        response.Content = new StringContent("{ error: \"No constituent records were found.\" }");
                        break;
                }
            }

            return response;
        }

        /// <summary>
        /// Returns constituent query parameters extracted from the next_link URL.
        /// </summary>
        /// <param name="nextLink">The next_link URL to extract params from.</param>
        public ListQueryParams CreateQueryParamsFromNextLinkUri(Uri nextLink)
        {
            var queryDictionary = HttpUtility.ParseQueryString(nextLink.Query);
            return new ListQueryParams
            {
                LastModified = queryDictionary["last_modified"],
                SortToken = queryDictionary["sort_token"]
            };
        }

        /// <summary>
        /// Builds a query string using the provided parameters.
        /// </summary>
        /// <param name="queryParams">The constituent query parameters to convert to a query string.</param>
        private string BuildQueryString(ListQueryParams queryParams)
        {
            if (queryParams == null)
            {
                return string.Empty;
            }

            var query = new List<string>();

            if (!string.IsNullOrEmpty(queryParams.LastModified))
            {
                query.Add($"last_modified={queryParams.LastModified}");
            }

            if (!string.IsNullOrEmpty(queryParams.SortToken))
            {
                query.Add($"sort_token={queryParams.SortToken}");
            }

            return string.Join('&', query);
        }
    }
}