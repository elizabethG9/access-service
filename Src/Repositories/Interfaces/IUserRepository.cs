using access_service.Src.Models;

namespace access_service.Src.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmail(string email);
        Task<User?> GetByRut(string rut);
        Task<User?> GetById(int id);
        Task<User> CreateUser(User user);
        Task<bool> UpdatePassword(int userId, string newPassword);
        


    }
}