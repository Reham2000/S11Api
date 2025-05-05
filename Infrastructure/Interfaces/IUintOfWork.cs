using Domin.DTOs;
using Domin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IUintOfWork : IDisposable

    {
        // new 
        UserManager<User> userManager { get; }
        RoleManager<IdentityRole> roleManager { get; }
        SignInManager<User> signInManager { get; }
        IHttpContextAccessor contextAccessor { get; }
        Jwt jwt { get; }





       IproductRepository products { get; }
        IRevokedTokenRepository revokedTokens { get; }
        IRefreshTokenRepository refreshTokens { get; }
        Task<int> SaveChangesAsync();
    }
   
}
