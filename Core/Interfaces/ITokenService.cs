using Domin.DTOs;
using Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(User user);

        // RefreshToken

        RefreshToken GenreateRefreshToken(string ipAddress,string jwtToken = null);
        Task<AuthenticateResponse> RefreshToken(string token, string ipAddress); 
        Task<bool> RevoceToken(string token, string ipAddress);
    }
}
