using AzureGallery.Models.EntityModels;
using AzureGallery.Models.HelperModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AzureGallery.Context
{
    public static class AzureGalleryContextExtensions
    {
        private static readonly string SeedPath = Path.Combine(Hosting.ContentRootPath, "Seed");


        public static void SeedData(this AzureGalleryContext context)
        {
            SeedUsers(context);

            context.SaveChanges();
        }

        private static void SeedUsers(AzureGalleryContext context)
        {
            string seedGendersPath = Path.Combine(SeedPath, "users.json");
            if (File.Exists(seedGendersPath))
            {
                context.Users.AddRange(JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(seedGendersPath)));
            }
        }
    }
}
