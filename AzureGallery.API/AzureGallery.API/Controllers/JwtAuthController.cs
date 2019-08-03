using AzureGallery.Models.DTOs;
using AzureGallery.Models.EntityModels;
using AzureGallery.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace AzureGallery.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class JwtAuthController : ControllerBase
    {
        IJwtAuthService _jwtAuthService;

        public JwtAuthController(IJwtAuthService jwtAuthService)
        {
            _jwtAuthService = jwtAuthService;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            userDTO.Username = userDTO.Username.ToLower();
            if (await _jwtAuthService.UserExist(userDTO.Username))
                return BadRequest("user already exists");

            var createdUser = await _jwtAuthService.Register(userDTO);

            return StatusCode((int)HttpStatusCode.Created, "User registered successfully");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userDTO)
        {
            var user = await _jwtAuthService.Login(userDTO.Username, userDTO.Password);
            if (user == null)
                return BadRequest("Invalid username or password!");

            var token = _jwtAuthService.CreateToken(user);

            return Ok(new { token, user = new UserDTO { Id = user.Id, Name = user.FullName } });
        }

        [HttpPost("GetUser")]
        public User GetUser([FromBody]string tokenstring)
        {
            return _jwtAuthService.GetLoggedUser(tokenstring);
        }

    }
}
