﻿using Domin.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

       public DbSet<Product> Products { get; set; }
       public DbSet<Category> Categories { get; set; }

        
    }
}
