using access_service.Src.DTOs;
using access_service.Src.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace access_service.Src.Controllers
{
    //aqui va login, register y update password
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginUserDto loginUserDto)
        {
            var result = await _authService.Login(loginUserDto);
            return Ok(result);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserDto registerUserDto)
        {
            var result = await _authService.Register(registerUserDto);
            return Ok(result);
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody]UpdatePasswordDto updatePasswordDto)
        {
            await _authService.UpdatePassword(updatePasswordDto);
            return Ok();
        }
    }
}