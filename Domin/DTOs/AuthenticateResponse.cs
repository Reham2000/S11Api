using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.DTOs
{
    public class AuthenticateResponse
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }

        public AuthenticateResponse(string jwtToken,string refreshToken)
        {
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }

    }
}
