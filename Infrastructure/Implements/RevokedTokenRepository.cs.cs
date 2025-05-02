using Domin.DTOs;
using Domin.Models;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implements
{
    public class RevokedTokenRepository : BaseRepository<RevokedToken>, IRevokedTokenRepository
    {
        private readonly AppDbContext _context;
        private readonly Jwt _jwt;
        public RevokedTokenRepository(AppDbContext context,IOptions<Jwt> jwt) : base(context)
        {
            _context = context;
            _jwt = jwt.Value;
        }

        public async Task<bool> IsTokenRevokedAsync(string jti)
        {
           return await _context.RevokedTokens.AnyAsync(r => r.Jti == jti);
        }

        public async Task RevokeRokenAsync(string jti)
        {
            await _context.RevokedTokens.AddAsync(new RevokedToken
            {
                Jti = jti,
                ExpirationDate = DateTime.Now.AddMinutes(_jwt.ExpiryDurationInMinutes)
            });
        }
    }
}
