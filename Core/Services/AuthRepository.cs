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
        //private readonly IUintOfWork _unitOfWork;
        //private readonly UserManager<User> _uintOfWork.userManager;
        //private readonly SignInManager<User> _signInManager;
        //private readonly IHttpContextAccessor _contextAccessor;
        //private readonly Jwt _jwt;
        //private readonly RoleManager<IdentityRole> _roleManager;
        //public AuthRepository(UserManager<User> userManager,SignInManager<User> signInManager,
        //    IHttpContextAccessor contextAccessor,IOptions<Jwt> jwt, IUintOfWork uintOfWork,
        //    RoleManager<IdentityRole> roleManager)
        //{
        //    _signInManager = signInManager;
        //    _uintOfWork.userManager = userManager;
        //    _contextAccessor = contextAccessor;
        //    _jwt = jwt.Value;
        //    _unitOfWork = uintOfWork;
        //    _roleManager = roleManager;
        //}
        // new 
        private readonly IUintOfWork _uintOfWork;
        public AuthRepository(IUintOfWork uintOfWork)
        {
            _uintOfWork = uintOfWork;
        }


        public async Task<ReturnModel<object>> RegisterAsync(RegisterDto model)
        {
            if(await UserExists(model.UserName))
                return new ReturnModel<object>
                {
                    IsSuccess = false,
                    Errors = new List<string> {"UserName Is Already Exist!" }
                };
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,

            };
            var result = await _uintOfWork.userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _uintOfWork.userManager.AddToRoleAsync(user, "User");
                return new ReturnModel<object>
                {
                    IsSuccess = true,
                    Data = new
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        Role = "User"
                    }
                };
            }
            return new ReturnModel<object>
            {
                IsSuccess = false,
                Errors = result.Errors.Select(r => r.Description).ToList()
            };


        }
        public async Task<ReturnModel<object>> AddAsync(UserDto model)
        {
            if (await UserExists(model.UserName))
                return new ReturnModel<object>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "UserName Is Already Exist!" }
                };
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,

            };
            var result = await _uintOfWork.userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var role = await _uintOfWork.roleManager.FindByIdAsync(model.RoleId);
                if (role is null)
                    return new ReturnModel<object>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Wrong RoleId!" }
                    };
                await _uintOfWork.userManager.AddToRoleAsync(user, role.Name);
                return new ReturnModel<object>
                {
                    IsSuccess = true,
                    Data = new
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        Role = role.Name
                    }
                };
            }
            return new ReturnModel<object>
            {
                IsSuccess = false,
                Errors = result.Errors.Select(r => r.Description).ToList()
            };


        }
        public async Task<ReturnModel<User>> LoginAsync(LoginDto model)
        {
            try
            {
                var user = await _uintOfWork.userManager.FindByNameAsync(model.UserName);
                if(user is not null && await _uintOfWork.userManager.CheckPasswordAsync(user, model.Password))
                {
                    var result = await _uintOfWork.signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        return new ReturnModel<User>
                        {
                            IsSuccess = true,
                            Data = user
                        };
                    }
                    return new ReturnModel<User>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Invaild UserName Or Password!"}
                    };
                }
                return new ReturnModel<User>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invaild UserName Or Password!" }
                };
            }
            catch (Exception ex)
            {
                return new ReturnModel<User>
                {
                    IsSuccess = false,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<bool> UserExists(string userName)
        {
            var user = await _uintOfWork.userManager.FindByNameAsync(userName);
            if (user != null)
                return true;
            return false;
        }
        public async Task StoreTokenAsync(User user, string token)
        {
            // identity token
            await _uintOfWork.userManager.SetAuthenticationTokenAsync(user,"JWT","Access_Token",token);
            // refresh token
            string userIpAddress = _uintOfWork.contextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var refreshToken = new RefreshToken
            {
                Token = token,
                Expires = DateTime.Now.AddMinutes(_uintOfWork.jwt.ExpiryDurationInMinutes),
                Created = DateTime.Now,
                CreatedByIp = userIpAddress,
                UserId = user.Id,
                //RevocedByIp = string.Empty,
            };
            await _uintOfWork.refreshTokens.AddAsync(refreshToken);
            await _uintOfWork.SaveChangesAsync();


        }
    }
}
