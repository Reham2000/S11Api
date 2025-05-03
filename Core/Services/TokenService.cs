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

namespace Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly Jwt _jwt;
        private readonly IUintOfWork _uintOfWork;
        private readonly UserManager<User> _userManager;
        public TokenService(IOptions<Jwt> jwt,IUintOfWork uintOfWork,UserManager<User> userManager)
        {
            _jwt = jwt.Value;
            _uintOfWork = uintOfWork;
            _userManager = userManager;
        }
        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // unique ID
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secretkey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var expires = DateTime.Now.AddMinutes(_jwt.ExpiryDurationInMinutes);
            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenreateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddMinutes(_jwt.ExpiryDurationInMinutes),
                Created = DateTime.Now,
                CreatedByIp = ipAddress,
                
            };
        }

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var refreshToken = await _uintOfWork.refreshTokens.GetByCriteriaAsync(r => r.Token == token);
            if (refreshToken == null || !refreshToken.IsActive)
                return null;
            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            // genertate new token = > refresh token
            var newRefreshToken = GenreateRefreshToken(ipAddress);
            // block old token
            // revoce it
            refreshToken.Revoced = DateTime.Now;
            refreshToken.RevocedByIp = ipAddress;

            // add & save
            user.RefreshTokens.Add(newRefreshToken); // add
            await _uintOfWork.SaveChangesAsync();  // save

            var jwtToken = GenerateJwtToken(user);
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
