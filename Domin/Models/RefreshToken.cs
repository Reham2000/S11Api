using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id {  get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }  // token expires in that dateTime
        public bool IsExpired =>  DateTime.Now >= Expires;  // not a database column
        public DateTime Created { get; set; } = DateTime.Now;
        public string CreatedByIp { get; set; }
        public DateTime? Revoced { get; set; } // block
        public string? RevocedByIp { get; set; }
        public bool IsActive =>  Revoced is null && !IsExpired;
        [ForeignKey("User")]
        public string UserId { get; set; }
        [ValidateNever]
        public User User { get; set; }
        
    }
}
