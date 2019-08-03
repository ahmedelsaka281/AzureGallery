
namespace AzureGallery.Services.IServices
{
    public interface IHasherService
    {
        string ComputeSha256Hash(string data);
    }
}
