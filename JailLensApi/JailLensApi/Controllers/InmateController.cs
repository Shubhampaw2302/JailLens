using JailLensApi.Data;
using JailLensApi.Infrastructure.IService;
using Microsoft.AspNetCore.Mvc;

namespace JailLensApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InmateController : ControllerBase
    {
        private readonly IInmateService _inmateService;

        public InmateController(IInmateService inmateService)
        {
            _inmateService = inmateService;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<int>> RegisterInmate(InmateImage inmate)
        {
            try
            {
                await _inmateService.RegisterInmate(inmate);
                return StatusCodes.Status200OK;
            }
            catch(Exception ex)
            {
                throw new Exception("Error while registering inmate", ex);
            }
        }

        [HttpPost]
        [Route("FindMatch")]
        public async Task<Inmate> FindMatchIfAny(Base64 base64)
        {
            try
            {
                return await _inmateService.FindMatch(base64.image);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while finding a match", ex);
            }
        }

        [HttpPost]
        [Route("Release")]
        public async Task<int> ReleaseInmate(ReleaseInmate inmateDetails)
        {
            try
            {
                await _inmateService.ReleaseInmate(inmateDetails);
                return StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while releasing inmate", ex);
            }
        }
    }
}
