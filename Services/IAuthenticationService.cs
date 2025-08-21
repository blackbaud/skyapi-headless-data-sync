using System.Net.Http;
using System.Threading.Tasks;

namespace Blackbaud.HeadlessDataSync.Services
{
    public interface IAuthenticationService
    {
        Task<HttpResponseMessage> RefreshAccessTokenAsync();
    }
}
