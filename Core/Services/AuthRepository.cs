using Core.Interfaces;
using Domin.DTOs;
using Domin.Models;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly Jwt _jwt;
        private readonly IUintOfWork _unitOfWork;
        public AuthRepository(UserManager<User> userManager,SignInManager<User> signInManager,
            IHttpContextAccessor contextAccessor,IOptions<Jwt> jwt, IUintOfWork uintOfWork)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _jwt = jwt.Value;
            _unitOfWork = uintOfWork;
        }



        public async Task<User> RegisterAsync(RegisterDto model)
        {
            if(await UserExists(model.UserName))
                return null;
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return user;
            }
            return null;


        }
        public async Task<User> LoginAsync(LoginDto model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if(user is not null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        return user;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UserExists(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
                return true;
            return false;
        }
        public async Task StoreTokenAsync(User user, string token)
        {
            // identity token
            await _userManager.SetAuthenticationTokenAsync(user,"JWT","Access_Token",token);
            // refresh token
            string userIpAddress = _contextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var refreshToken = new RefreshToken
            {
                Token = token,
                Expires = DateTime.Now.AddMinutes(_jwt.ExpiryDurationInMinutes),
                Created = DateTime.Now,
                CreatedByIp = userIpAddress,
                UserId = user.Id,
                //RevocedByIp = string.Empty,
            };
            await _unitOfWork.refreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();


        }
    }
}
