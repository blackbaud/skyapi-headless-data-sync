using System.Net.Http;

namespace Blackbaud.HeadlessDataSync.Services
{
    public interface IAuthenticationService
    {   
        HttpResponseMessage RefreshAccessToken();
    }
}