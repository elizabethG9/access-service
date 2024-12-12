using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using access_service.Src.DTOs;
using access_service.Src.Models;
using access_service.Src.Repositories.Interfaces;
using access_service.Src.Services.Interfaces;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;

namespace access_service.Src.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
    
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;

        }
        public async Task<LoggedUserDto> Login(LoginUserDto loginUserDto)
        {
            //verificar que el correo y contraseña sean correctos
            var user = await _userRepository.GetByEmail(loginUserDto.Email) ?? throw new Exception("Usuario no encontrado");

            var verifyPassword = BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.Password); 
            if (!verifyPassword) throw new Exception("Contraseña invalida");

            var token = CreateToken(user);

            var loggedUser = new LoggedUserDto
            {
                Token = token
            };
  
            return loggedUser;
        }

        public async Task<LoggedUserDto> Register(RegisterUserDto registerUserDto)
        {
            // Verificar que ni el rut ni el email estén en el sistema
            await ValidateEmailAndRut(registerUserDto.Email, registerUserDto.Rut);

            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            // Crear un nuevo usuario
            var user = new User
            {
                Email = registerUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password, salt),
                Rut = registerUserDto.Rut
            };

            // Guardar el usuario en el repositorio
            var createdUser = await _userRepository.CreateUser(user);

            // Generar un JWT para el usuario registrado
            var token = CreateToken(createdUser);

            // Devolver un DTO con la información del usuario y el token
            return new LoggedUserDto
            {
                Token = token
            };
        }
        public async Task UpdatePassword(UpdatePasswordDto updatePasswordDto, int userId)
        {
            //verificar que la contraseña sea correcta y que la nueva y la confirmacion de la password sean iguales
            var user = await _userRepository.GetById(userId);
            if (user == null) throw new Exception("User not found");

            var verifyPassword = BCrypt.Net.BCrypt.Verify(updatePasswordDto.CurrentPassword, user.Password);
            if (!verifyPassword) throw new Exception("Invalid password");

            if (updatePasswordDto.NewPassword != updatePasswordDto.ConfirmNewPassword){
                throw new Exception("Las contraseñas no coinciden");
            }
            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            user.Password = BCrypt.Net.BCrypt.HashPassword(updatePasswordDto.NewPassword, salt);

            await _userRepository.UpdatePassword(userId, user.Password);
        }
        private async Task ValidateEmailAndRut(string email, string rut)
        {
            var userByEmail = await _userRepository.GetByEmail(email);
            if (userByEmail != null) throw new Exception("El email ya existe en el sistema");

            var userByRut = await _userRepository.GetByRut(rut);
            if (userByRut != null) throw new Exception("El Rut ya existe en el sistema");
        }
        private string CreateToken(User user)
        {
            Env.Load();
            var secret = Env.GetString("JWT_SECRET");

            var claims = new List<Claim>{
                new ("Id", user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    secret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        
    }
}