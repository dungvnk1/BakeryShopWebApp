using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KingBakery.Models;

namespace KingBakery.Data
{
    public class KingBakeryContext : DbContext
    {
        public KingBakeryContext (DbContextOptions<KingBakeryContext> options)
            : base(options)
        {
        }

        public DbSet<KingBakery.Models.Users> Users { get; set; } = default!;
        public DbSet<KingBakery.Models.Employee> Employee { get; set; } = default!;
        public DbSet<KingBakery.Models.Customer> Customer { get; set; } = default!;
    }
}
