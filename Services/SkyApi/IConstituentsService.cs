using Blackbaud.HeadlessDataSync.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blackbaud.HeadlessDataSync.Services.SkyApi
{
    public interface IConstituentsService
    {
        Task<HttpResponseMessage> GetConstituentsAsync(ListQueryParams queryParams);

        ListQueryParams CreateQueryParamsFromNextLinkUri(Uri nextLink);
    }
}
