using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Domin.Models
{
    public class User : IdentityUser
    {
        [ValidateNever]
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
