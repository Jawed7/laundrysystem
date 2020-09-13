using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using laundry.Models;

namespace laundry.Data
{
    public class laundryNewDbContext : DbContext
    {
        public laundryNewDbContext (DbContextOptions<laundryNewDbContext> options)
            : base(options)
        {
        }

        public DbSet<laundry.Models.Customers> Customers { get; set; }
    }
}
