using Infrastructure.Data;
using Infrastructure.Interfaces;
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
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            products = new ProductRepository(context);
        }

        public IproductRepository products { get; private set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();


    }
}
