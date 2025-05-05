using Core.Interfaces;
using Domin.DTOs;
using Domin.Models;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class TokenService : ITokenService
    {
        //private readonly Jwt _uintOfWork.jwt;
        private readonly IUintOfWork _uintOfWork;
        //private readonly UserManager<User> _uintOfWork.userManager;
        public TokenService(/*IOptions<Jwt> jwt,*/IUintOfWork uintOfWork/*,UserManager<User> userManager*/)
        {
            //_uintOfWork.jwt = jwt.Value;
            _uintOfWork = uintOfWork;
            //_uintOfWork.userManager = userManager;
        }
        public async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // unique ID
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };
            var userRoles = await _uintOfWork.userManager.GetRolesAsync(user);
            foreach (var role in userRoles) {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_uintOfWork.jwt.Secretkey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var expires = DateTime.Now.AddMinutes(_uintOfWork.jwt.ExpiryDurationInMinutes);
            var token = new JwtSecurityToken(
                issuer: _uintOfWork.jwt.Issuer,
                audience: _uintOfWork.jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenreateRefreshToken(string ipAddress,string jwtToken = null)
        {
            return new RefreshToken
            {
                Token = jwtToken == null ?
                Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)) 
                : jwtToken,
                Expires = DateTime.Now.AddMinutes(_uintOfWork.jwt.ExpiryDurationInMinutes),
                Created = DateTime.Now,
                CreatedByIp = ipAddress,
                
            };
        }

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var refreshToken = await _uintOfWork.refreshTokens.GetByCriteriaAsync(r => r.Token == token);
            if (refreshToken == null || !refreshToken.IsActive)
                return null;
            var user = await _uintOfWork.userManager.FindByIdAsync(refreshToken.UserId);
            var jwtToken = await GenerateJwtToken(user);
            // genertate new token = > refresh token
            var newRefreshToken = GenreateRefreshToken(ipAddress,jwtToken);
            // block old token
            // revoce it
            refreshToken.Revoced = DateTime.Now;
            refreshToken.RevocedByIp = ipAddress;

            // add & save
            user.RefreshTokens.Add(newRefreshToken); // add
            await _uintOfWork.SaveChangesAsync();  // save

            return new AuthenticateResponse(jwtToken,newRefreshToken.Token);

        }

        public async Task<bool> RevoceToken(string token, string ipAddress)
        {
            var refreshToken = await _uintOfWork.refreshTokens.GetByCriteriaAsync(r => r.Token == token);
            if(refreshToken == null || !refreshToken.IsActive)
                return false;

            // update in refresh token and revoce it
            refreshToken.Revoced = DateTime.Now;
            refreshToken.RevocedByIp = ipAddress;
            // then save
            await _uintOfWork.SaveChangesAsync();
            return true;




            
        }
    }
}
