using Microsoft.AspNetCore.Hosting;

namespace AzureGallery.Models.HelperModels
{
    public static class Hosting
    {
        public static IHostingEnvironment Environment { get; set; }
        public static string ContentRootPath => Environment.ContentRootPath;
    }
}
