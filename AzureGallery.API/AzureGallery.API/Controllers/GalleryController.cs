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
        public IIOService _iOService;

        public GalleryController(IAzureService azureService, IIOService iOService)
        {
            _azureService = azureService;
            _iOService = iOService;
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


        [HttpGet("DownloadFile/{name}")]
        public async Task<FileResult> DownloadFile(string name)
        {
            FileContentResult result = null;
            if (!string.IsNullOrEmpty(name))
            {
                string path = await _azureService.DownloadFileAsync(name);
                byte[] binaryContent = System.IO.File.ReadAllBytes(path);
                string fileName = System.IO.Path.GetFileName(path);
                string mimeType = _iOService.GetMimeTypeByWindowsRegistry(fileName);

                if (binaryContent != null)
                {
                    result = new FileContentResult(binaryContent, mimeType)
                    {
                        FileDownloadName = fileName,
                    };
                }
            }
            return result;
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
