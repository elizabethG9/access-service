
using access_service.Src.Models;
using Microsoft.EntityFrameworkCore;

namespace access_service.Src.Data
{

    public class DataContext : DbContext
    {

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Career> Careers { get; set; } = null!;

        public DataContext(DbContextOptions options) : base(options) { }

        // Configuración de las entidades si es necesario
    }
}