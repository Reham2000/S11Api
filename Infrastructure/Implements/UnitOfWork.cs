using Domin.DTOs;
using Domin.Models;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implements
{
    public class UnitOfWork : IUintOfWork
    {
        private readonly AppDbContext _context;
        private readonly Jwt _jwt;


        // new 
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public UnitOfWork(AppDbContext context ,IOptions<Jwt> jwt,
            UserManager<User> userManager,SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _jwt = jwt.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
            products = new ProductRepository(context);
            revokedTokens = new RevokedTokenRepository(context,jwt);
            refreshTokens = new RefreshTokenRepository(context);
        }

        public UserManager<User> userManager => _userManager;
        public SignInManager<User> signInManager => _signInManager;
        public RoleManager<IdentityRole> roleManager => _roleManager;
        public IHttpContextAccessor contextAccessor => _contextAccessor;
        public Jwt jwt => _jwt;


        public IproductRepository products { get; private set; }
        public IRevokedTokenRepository revokedTokens { get; private set; }
        public IRefreshTokenRepository refreshTokens { get; private set; }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();


    }
}
