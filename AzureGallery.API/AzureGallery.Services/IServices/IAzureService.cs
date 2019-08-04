using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureGallery.Services.IServices
{
    public interface IAzureService
    {
        Task<List<string>> GetAllFilesAsync();
        Task<bool> UploadFileAsync(IFormFile file);
        Task<string> DownloadFileAsync(string fileName);
        Task<bool> DeleteFileAsync(string fileName);
        Task<bool> DeleteAllFilesAsync();
    }
}
