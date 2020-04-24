using System.Threading.Tasks;

namespace Blackbaud.HeadlessDataSync.Services.DataSync
{
    public interface IDataSyncService
    {
        Task<bool> SyncConstituentData();

        // additional sync processes...
    }
}
