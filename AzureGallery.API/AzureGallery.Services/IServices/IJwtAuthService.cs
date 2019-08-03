using AzureGallery.Models.DTOs;
using AzureGallery.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureGallery.Services.IServices
{
    public interface IJwtAuthService
    {
        Task<User> Register(UserRegisterDTO user);
        Task<User> Login(string username, string password);
        Task<bool> UserExist(string username);
        string CreateToken(User user);
        User GetLoggedUser(string tokenstring);
    }
}
