﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IServiceUnitOfWork
    {
        public IAuthRepository authRepository { get; }
        public ITokenService tokenService { get; }
        public IProductService productService { get; }
    }
}
