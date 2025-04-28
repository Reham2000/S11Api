using Domin.DTOs;
using Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(ProductDTo model);
        Task UpdateAsync(Product model);
        Task DeleteAsync(int id);

    }
}
