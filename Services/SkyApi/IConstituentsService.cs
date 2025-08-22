using Blackbaud.HeadlessDataSync.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Blackbaud.HeadlessDataSync.Services.SkyApi
{
    public interface IConstituentsService
    {
        Task<HttpResponseMessage> GetConstituentsAsync(ListQueryParams queryParams, CancellationToken cancellationToken = default);

        ListQueryParams CreateQueryParamsFromNextLinkUri(Uri nextLink);
    }
}
