using Domin.DTOs;
using Domin.Models;

namespace Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> RegisterAsync(RegisterDto model);
        Task<User> LoginAsync(LoginDto model);
        Task<bool> UserExists(string userName);
        Task StoreTokenAsync(User user, string token);
    }
}
