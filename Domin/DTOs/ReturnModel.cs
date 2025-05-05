using Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.DTOs
{
    public class ReturnModel<T>
    {
        public bool IsSuccess { get; set; }
        public List<string>? Errors { get; set; }
        public T? Data { get; set; }

    }
}
