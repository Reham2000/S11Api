using Domin.DTOs;
using Infrastructure.Data;
using Infrastructure.Interfaces;
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
        private readonly IOptions<Jwt> _jwt;
        public UnitOfWork(AppDbContext context ,IOptions<Jwt> jwt)
        {
            _context = context;
            _jwt = jwt;
            products = new ProductRepository(context);
            revokedTokens = new RevokedTokenRepository(context,_jwt);
        }

        public IproductRepository products { get; private set; }
        public IRevokedTokenRepository revokedTokens { get; private set; }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();


    }
}
