using JailLensApi.Data;
using JailLensApi.Data.Models;
using JailLensApi.Infrastructure.IService;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace JailLensApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceController : ControllerBase
    {
        private readonly IFaceService _faceService;

        public FaceController(IFaceService faceService)
        {
            _faceService = faceService;
        }

        [HttpGet]
        [Route("GetFacialEvents")]
        public async Task<List<FacialEvent>> GetFacialEvents()
        {
            try
            {
                return await _faceService.GetFacialEvents();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting facial events : ", ex);
            }
        }

        [HttpPost]
        [Route("ImageUpload")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return BadRequest("No image file specified.");
                }

                string uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                // Generate a unique file name for the uploaded image
                string fileName = Path.GetFileNameWithoutExtension(image.FileName);
                string fileExtension = Path.GetExtension(image.FileName);
                string uniqueFileName = $"{fileName}_{Path.GetRandomFileName()}.{fileExtension}";

                // Save the uploaded image to the server
                string filePath = Path.Combine(uploadsDirectory, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                return Ok(new { fileName = uniqueFileName, filePath });
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting facial events : ", ex);
            }
        }

        [HttpPost]
        [Route("AddFacialEvents")]
        public async Task<int> AddFacialEvents(FaceRecognitionEvents faceEvent)
        {
            try
            {
                var result = await _faceService.AddFacialEvents(faceEvent);

                if (result == "Success")
                {
                    return StatusCodes.Status200OK;
                }

                return StatusCodes.Status500InternalServerError;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding facial events : ", ex);
            }
        }

        [HttpPost]
        [Route("ProcessFaceEvents")]
        public async Task ProcessFaceEvents() 
        {
            try
            {
                await _faceService.ProcessFacialEvents();
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong ", ex);
            }
        }
    }
}
