using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using access_service.Src.Validations;
namespace access_service.Src.DTOs
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "El campo Nombre es requerido")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "El campo Apellido Paterno es requerido")]
        public string FirstLastName { get; set; } = null!;
        [Required(ErrorMessage = "El campo Apellido Materno es requerido")]
        public string SecondLastName { get; set; } = null!;
        [RutValidation(ErrorMessage = "El Rut entregado no es válido")]        
        public string Rut { get; set; } = null!;

        [UCNEmailAddress(ErrorMessage = "El Email entregado no es válido")]
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public int CareerId { get; set; } 

    }
    
}