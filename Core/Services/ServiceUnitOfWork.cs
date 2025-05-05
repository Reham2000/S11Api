using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class ServiceUnitOfWork : IServiceUnitOfWork
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IProductService _productService;
        public ServiceUnitOfWork(IAuthRepository authRepository,
            ITokenService tokenService, IProductService productService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _productService = productService;
        }


        public IAuthRepository authRepository => _authRepository;
        public ITokenService tokenService => _tokenService;
        public IProductService productService => _productService;
    }
}
