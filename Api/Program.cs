using Core.Authorization;
using Core.Interfaces;
using Core.Middelware;
using Core.Services;
using Domin.DTOs;
using Domin.Models;
using Infrastructure.Data;
using Infrastructure.Implements;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("con")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUintOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();
builder.Services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();




// add authorization policy

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.Requirements.Add( 
        new CustomAuthorizationRequrement(new List<string> { "Admin"})
            ));
    options.AddPolicy("ManagerPolicy", policy => policy.Requirements.Add(
        new CustomAuthorizationRequrement(new List<string> { "Manager" })
            ));
    options.AddPolicy("UserPolicy", policy => policy.Requirements.Add(
        new CustomAuthorizationRequrement(new List<string> { "User" })
            ));
    options.AddPolicy("AdminManagerPolicy", policy => policy.Requirements.Add(
        new CustomAuthorizationRequrement(new List<string> { "Admin" ,"Manager" })
            ));
    options.AddPolicy("UserManagerPolicy", policy => policy.Requirements.Add(
        new CustomAuthorizationRequrement(new List<string> { "User", "Manager" })
            ));
    options.AddPolicy("AllPolicy", policy => policy.Requirements.Add(
        new CustomAuthorizationRequrement(new List<string> { "Admin","User","Manager" })
            ));
});







builder.Services.Configure<Jwt>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<Jwt>();
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secretkey)),
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,

    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var revocedTokenService = context.HttpContext.RequestServices
            .GetRequiredService<IRevokedTokenRepository>();

            var jti = context.Principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (jti is not null && await revocedTokenService.IsTokenRevokedAsync(jti))
                context.Fail("this token has been revoced!");

        }
    };
});









// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using(var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    await DbInitializer.SeedRoles(service);
}




app.UseMiddleware<TokenRevocation>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
