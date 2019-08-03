using System.Net;
using System.Threading.Tasks;
using AzureGallery.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureGallery.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GalleryController : ControllerBase
    {
        public IAzureService _azureService;

        public GalleryController(IAzureService azureService)
        {
            _azureService = azureService;
        }


        [HttpGet("GetAllFiles")]
        public async Task<ActionResult> GetAllFiles()
        {
            return Ok(await _azureService.GetAllFilesAsync());
        }

        [HttpPost("UploadFile")]
        public async Task<ActionResult> UploadFile()
        {
            var file = Request.Form.Files[0];

            var res = await _azureService.UploadFileAsync(file);

            if (res)
                return Ok("uploaded Successfully");

            return StatusCode((int)HttpStatusCode.InternalServerError, "Error When Upload File!");
        }

        [HttpPost("DownloadFile/{fileName}")]
        public async Task<ActionResult> DownloadFile(string fileName)
        {
            var res = await _azureService.DownloadFileAsync(fileName);

            //var memory = new MemoryStream();
            //using (var stream = new FileStream(res, FileMode.Open))
            //{
            //    await stream.CopyToAsync(memory);
            //}
            //memory.Position = 0;

            //return File(memory, GetContentType(res), Guid.NewGuid() + ".jpg");

            if (res)
                return Ok("Downloaded Successfully");

            return StatusCode((int)HttpStatusCode.InternalServerError, "Error When Download File!");
        }

        [HttpDelete("DeleteFile/{fileName}")]
        public async Task<ActionResult> DeleteFile(string fileName)
        {
            bool res = await _azureService.DeleteFileAsync(fileName);

            if (res)
                return Ok("Deleted Successfully");

            return StatusCode((int)HttpStatusCode.InternalServerError, "Error When Delete File!");
        }

        [Authorize]
        [HttpDelete("DeleteAllFiles")]
        public async Task<ActionResult> DeleteAllFiles()
        {
            bool res = await _azureService.DeleteAllFilesAsync();

            if (res)
                return Ok("Deleted Successfully");

            return StatusCode((int)HttpStatusCode.InternalServerError, "Error When Delete Files!");
        }

    }
}
