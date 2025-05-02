using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.Models
{
    public class RevokedToken
    {
        public int Id { get; set; }
        public string Jti { get; set; } // unique token ID
        public DateTime ExpirationDate { get; set; }
    }
}
