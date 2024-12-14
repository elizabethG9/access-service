using access_service.Src.DTOs;
using access_service.Src.Helpers;
using access_service.Src.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Messages;

namespace access_service.Src.Controllers
{
    //aqui va login, register y update password
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IBlackListService _blackListService;

        private readonly IPublishEndpoint _publishEndpoint;
        
        public AuthController(IAuthService authService, IBlackListService blackListService, IPublishEndpoint publishEndpoint)
        {
            _authService = authService;
            _blackListService = blackListService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginUserDto loginUserDto)
        {
            try
            {
                var result = await _authService.Login(loginUserDto);
                return Ok(result);
            }
            catch (BadRequestException BadRequest)
            {
                throw new BadRequestException(BadRequest.Message);
            }

        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserDto registerUserDto)
        {
            try
            {
                var result = await _authService.Register(registerUserDto);
                return Ok(result);
            }
            catch (BadRequestException BadRequest)
            {
                throw new BadRequestException(BadRequest.Message);
            }
            
        }

        [Authorize]
        [HttpPatch("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody]UpdatePasswordDto updatePasswordDto)
        {
            try
            {
                await _authService.UpdatePassword(updatePasswordDto, 1);
                // Add token to blacklist
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                _blackListService.AddToBlacklist(token);
                var message = new TokenToBlacklistMessage
                {
                    Token = token
                };
                await _publishEndpoint.Publish(message);

                return Ok();
            }
            catch (BadRequestException BadRequest)
            {
                throw new BadRequestException(BadRequest.Message); 
            }
        }
    }
}