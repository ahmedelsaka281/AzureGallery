using AzureGallery.Services.IServices;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace AzureGallery.Services.Services
{
    public class IOService : IIOService
    {
        public async Task<bool> WriteFileAsync(IFormFile file, string filePath)
        {
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    return true;
                }
            }
            catch { return false; }
        }

        public void CreateDirectoryIfNotExist(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        public void DeleteFileIfExist(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public string GetMimeTypeByWindowsRegistry(string filePath)
        {
            string mimeType = "application/unknown";
            string ext = (filePath.Contains(".")) ? System.IO.Path.GetExtension(filePath).ToLower() : "." + filePath;
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null) mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }
}
