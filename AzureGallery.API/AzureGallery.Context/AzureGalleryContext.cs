using AzureGallery.Models.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace AzureGallery.Context
{
    public class AzureGalleryContext : DbContext
    {
        public AzureGalleryContext(DbContextOptions<AzureGalleryContext> options) : base(options)
        {
            Database.EnsureCreated();
        }


        public DbSet<User> Users { get; set; }

    }
}
