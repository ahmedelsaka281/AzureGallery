using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AzureGallery.Services.IServices
{
    public interface IIOService
    {
        Task<bool> WriteFileAsync(IFormFile file, string filePath);
        void CreateDirectoryIfNotExist(string dirPath);
        void DeleteFileIfExist(string filePath);
    }
}
