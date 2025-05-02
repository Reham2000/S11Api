﻿using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Middelware
{
    public class TokenRevocation
    {
        private readonly RequestDelegate _next;
        public TokenRevocation(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUintOfWork unitOfWork)
        {
            // get jti
            var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (!string.IsNullOrWhiteSpace(jti)) { 
                var isRevoced = await unitOfWork.revokedTokens.IsTokenRevokedAsync(jti);
                if (isRevoced) {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        message = "Token has been revoced!"
                    });
                    return;
                }
            }
            await _next(context);
        }


    }
}
