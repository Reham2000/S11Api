using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Authorization
{
    public class CustomAuthorizationRequrement : IAuthorizationRequirement
    {
        public List<string> AllowedRoles { get; }
        public CustomAuthorizationRequrement(List<string> allowedRoles)
        {
            AllowedRoles = allowedRoles ?? new List<string>();
        }
    }
}
