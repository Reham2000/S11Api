using Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IRevokedTokenRepository : IBaseRepository<RevokedToken>
    {
        Task<bool> IsTokenRevokedAsync(string jti);
        Task RevokeRokenAsync(string jti);
    }
}
