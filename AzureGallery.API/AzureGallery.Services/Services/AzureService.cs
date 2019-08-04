using AzureGallery.Models.HelperModels;
using AzureGallery.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureGallery.Services.Services
{
    public class AzureService : IAzureService
    {
        private readonly IConfiguration _configuration;
        private readonly IIOService _iOService;

        private readonly CloudStorageAccount storageAccount;
        private readonly CloudBlobClient cloudBlobClient;
        private readonly CloudBlobContainer cloudBlobContainer;

        public AzureService(IConfiguration configuration, IIOService iOService)
        {
            _configuration = configuration;
            _iOService = iOService;
            if (CloudStorageAccount.TryParse(_configuration["AzureSettings:AccountConnectionString"], out storageAccount))
            {
                cloudBlobClient = storageAccount.CreateCloudBlobClient();
                cloudBlobContainer = cloudBlobClient.GetContainerReference(_configuration["AzureSettings:ContainerName"]);
            }
        }


        public async Task<List<string>> GetAllFilesAsync()
        {
            List<string> res = new List<string>();
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                blobContinuationToken = results.ContinuationToken;

                foreach (IListBlobItem item in results.Results)
                {
                    res.Add(item.Uri.ToString());
                }
            } while (blobContinuationToken != null); // Loop while the continuation token is not null.
            return res;
        }

        public async Task<bool> UploadFileAsync(IFormFile file)
        {
            try
            {
                var dir = Path.Combine(Hosting.ContentRootPath, "wwwroot", "Temp");
                _iOService.CreateDirectoryIfNotExist(dir);

                var path = Path.Combine(dir, file.FileName);

                //save file in serve to get it's physical path
                if (await _iOService.WriteFileAsync(file, path))
                {
                    //upload saved file to azure
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
                    await cloudBlockBlob.UploadFromFileAsync(path);

                    //delete temp file after upload it to azure
                    _iOService.DeleteFileIfExist(path);

                    return true;
                }
            }
            catch { }
            return false;
        }

        public async Task<string> DownloadFileAsync(string fileName)
        {
            try
            {
                var dir = Path.Combine(Hosting.ContentRootPath, "wwwroot", "Downloads");
                _iOService.CreateDirectoryIfNotExist(dir);

                string destination = Path.Combine(dir, Guid.NewGuid() + fileName);

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                await cloudBlockBlob.DownloadToFileAsync(destination, FileMode.Create);

                return destination;
            }
            catch { return null; }
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                return await cloudBlockBlob.DeleteIfExistsAsync();
            }
            catch { return false; }
        }

        public async Task<bool> DeleteAllFilesAsync()
        {
            try
            {
                BlobContinuationToken blobContinuationToken = null;
                do
                {
                    var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    // Get the value of the continuation token returned by the listing call.
                    blobContinuationToken = results.ContinuationToken;
                    foreach (IListBlobItem item in results.Results)
                    {
                        CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(((CloudBlob)item).Name);
                        await cloudBlockBlob.DeleteIfExistsAsync();
                    }
                } while (blobContinuationToken != null); // Loop while the continuation token is not null.
                return true;
            }
            catch { return false; }
        }

    }
}
