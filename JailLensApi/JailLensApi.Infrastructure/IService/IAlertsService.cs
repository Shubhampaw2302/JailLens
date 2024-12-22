using JailLensApi.Data;
using JailLensApi.Data.Models;

namespace JailLensApi.Infrastructure.IService
{
    public interface IAlertsService
    {
        Task<List<JailLensAlert>> GetAlerts();
        Task ProcessAlert(JailLensAlert alert);
        Task<int> GetAlertsCount();
        Task AddAbsenteeismAlert(int programId);
        Task<List<ProgramEndResponse>> GetPrograms();
    }
}
