using Blackbaud.HeadlessDataSync.Models;
using System;
using System.Net.Http;

namespace Blackbaud.HeadlessDataSync.Services.SkyApi
{
    public interface IConstituentsService
    {
        HttpResponseMessage GetConstituents(ListQueryParams queryParams);

        ListQueryParams CreateQueryParamsFromNextLinkUri(Uri nextLink);
    }
}