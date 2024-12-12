namespace access_service.Src.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Rut { get; set; } = null!;
        public string Email { get; set; }  = null!;
        public string Password { get; set; } = null!;
    }
    
}