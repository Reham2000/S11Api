using Domin.DTOs;
using Domin.Models;

namespace Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<ReturnModel<object>> RegisterAsync(RegisterDto model);
        Task<ReturnModel<object>> AddAsync(UserDto model);
        Task<ReturnModel<User>> LoginAsync(LoginDto model);
        Task<bool> UserExists(string userName);
        Task StoreTokenAsync(User user, string token);
    }
}
