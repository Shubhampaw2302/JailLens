using JailLensApi.Data;

namespace JailLensApi.Infrastructure.IService
{
    public interface IInmateService
    {
        Task RegisterInmate(InmateImage inmate);
        Task<Inmate> FindMatch(string base64Image);
        Task ReleaseInmate(ReleaseInmate inmateDetails);
    }
}
