using Domin.Models;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Implements
{
    public class ProductRepository : BaseRepository<Product>, IproductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }
    }
    
}
