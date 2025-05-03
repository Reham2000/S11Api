using Domin.Models;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implements
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly AppDbContext _context;
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<RefreshToken> GetActiveRefreshTokenAsync(string token)
        {
            var res = await _context.RefreshTokens.Where(r => r.Token == token).ToListAsync();
            return  res.FirstOrDefault(r => r.IsActive);
        }
    }
}
