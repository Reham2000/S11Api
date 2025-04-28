using Domin.Models;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implements
{
    public class ProductRepository : BaseRepository<Product>, IproductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }
    }
    
}
