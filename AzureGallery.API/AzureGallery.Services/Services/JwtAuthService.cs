using AutoMapper;
using AzureGallery.Context;
using AzureGallery.Models.DTOs;
using AzureGallery.Models.EntityModels;
using AzureGallery.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AzureGallery.Services.Services
{
    public class JwtAuthService : IJwtAuthService
    {
        private readonly AzureGalleryContext _context;
        private readonly IMapper _iMapper;
        private readonly IHasherService _hasherService;
        private readonly IConfiguration _configuration;

        public JwtAuthService(AzureGalleryContext context, IMapper mapper, IConfiguration configuration, IHasherService hasherService)
        {
            _context = context;
            _iMapper = mapper;
            _configuration = configuration;
            _hasherService = hasherService;
        }


        public async Task<User> Register(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                var user = _iMapper.Map<UserRegisterDTO, User>(userRegisterDTO);
                //HASH PASSWORD
                user.PasswordHash = _hasherService.ComputeSha256Hash(userRegisterDTO.Password);

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch { return null; }
        }

        public async Task<User> Login(string username, string password)
        {
            var current = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if(current != null)
            {
                if(current.PasswordHash == _hasherService.ComputeSha256Hash(password))
                {
                    return current;
                }
            }
            return null;
        }

        public string CreateToken(User user)
        {
            Claim[] calims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), //nameid
                new Claim(ClaimTypes.Name, user.Username), //uniquename
            };

            SymmetricSecurityKey SignatureKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWTSettings:SignatureKey"]));
            SigningCredentials creds = new SigningCredentials(SignatureKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(calims),
                SigningCredentials = creds,
                Expires = DateTime.UtcNow.AddHours(12),
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public User GetLoggedUser(string tokenString)
        {
            if (!string.IsNullOrEmpty(tokenString))
            {
                var jwtEncodedString = tokenString.Substring(7);
                var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                var nameid = token.Claims.First(c => c.Type == "nameid").Value;
                int userId;
                if (int.TryParse(nameid, out userId))
                {
                    return _context.Users.FirstOrDefault(x => x.Id == userId);
                }
            }
            return null;
        }

        public async Task<bool> UserExist(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username))
                return true;
            return false;
        }
    }
}
