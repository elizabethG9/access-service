using access_service.Src.Validations;

namespace access_service.Src.DTOs
{
    public class LoginUserDto
    {
        [UCNEmailAddress(ErrorMessage = "El email entregado no es válido")]
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

    }
    
}