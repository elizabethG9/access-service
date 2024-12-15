using access_service.Src.Data;
using access_service.Src.Models;
using access_service.Src.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace access_service.Src.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        }

        public Task<User?> GetById(int id)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByRut(string rut)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Rut == rut);
        }

        public async Task<bool> UpdatePassword(int userId, string newPassword)
        {
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            user.Password = newPassword;

            await _context.SaveChangesAsync();
            return true;
        }

    }
}