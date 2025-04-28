using Core.Interfaces;
using Domin.DTOs;
using Domin.Models;
using Infrastructure.Interfaces;

namespace Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IUintOfWork _unitOfWork;
        public ProductService(IUintOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = await _unitOfWork.products.GetAllAsync();
            if (products == null || !products.Any())
            {
                throw new Exception("No products found");
            }
            return products;
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.products.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception($"Product with id {id} not found");
            }
            return product;
        }
        public async Task AddAsync(ProductDTo model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Product model cannot be null");
            }
            var productdata = new Product
            {
                CategoryId = model.CategoryId,
                Name = model.Name,
                Price = model.Price,
                
                
            };
            await _unitOfWork.products.AddAsync(productdata);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateAsync(Product model)
        {
            var product = await _unitOfWork.products.GetByIdAsync(model.Id);
            if (product == null)
            {
                throw new Exception($"Product with id {model.Id} not found");
            }
            product.CategoryId = model.CategoryId;
            product.Name = model.Name;
            product.Price = model.Price;
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _unitOfWork.products.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception($"Product with id {id} not found");
            }
            await _unitOfWork.products.DeleteAsync(id);
            //await _unitOfWork.products.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
