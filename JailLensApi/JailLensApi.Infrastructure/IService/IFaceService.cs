using JailLensApi.Data;
using JailLensApi.Data.Models;

namespace JailLensApi.Infrastructure.IService
{
    public interface IFaceService
    {
        Task<List<FacialEvent>> GetFacialEvents();
        Task<string> AddFacialEvents(FaceRecognitionEvents faceEvent);
        Task ProcessFacialEvents();
    }
}
