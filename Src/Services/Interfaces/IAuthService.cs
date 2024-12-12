using access_service.Src.DTOs;

namespace access_service.Src.Services.Interfaces
{
    public interface IAuthService
    {
        public Task <LoggedUserDto> Login(LoginUserDto loginUserDto);
        public Task <LoggedUserDto> Register(RegisterUserDto registerUserDto);
        public Task UpdatePassword(UpdatePasswordDto updatePasswordDto);
    }
}