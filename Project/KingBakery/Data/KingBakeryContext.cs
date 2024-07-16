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
        public DbSet<KingBakery.Models.Bakery> Bakery { get; set; } = default!;
        public DbSet<KingBakery.Models.BakeryOption> BakeryOption { get; set; } = default!;
        public DbSet<KingBakery.Models.Category> Category { get; set; } = default!;
        public DbSet<KingBakery.Models.Vouchers> Vouchers { get; set; } = default!;
        public DbSet<KingBakery.Models.Orders> Orders { get; set; } = default!;
        public DbSet<KingBakery.Models.OrderItem> OrderItem { get; set; } = default!;
        public DbSet<KingBakery.Models.Feedback> Feedback { get; set; } = default!;
        public DbSet<KingBakery.Models.Favourite> Favourite { get; set; } = default!;
        public DbSet<KingBakery.Models.BlogPosts> BlogPosts { get; set; } = default!;
        //public DbSet<KingBakery.Models.Shipper> Shipper { get; set; } = default!;
        public DbSet<KingBakery.Models.Staff> Staff { get; set; } = default!;


        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<Favourite>().HasKey(p => new { p.BakeryID, p.CustomerID });
        //    //builder.Entity<Orders>()
        //    //    .HasOne<Employee>()
        //    //    .WithMany()
        //    //    .HasForeignKey(o => o.StaffID);
        //    //builder.Entity<Orders>()
        //    //    .HasOne<Employee>()
        //    //    .WithMany()
        //    //    .HasForeignKey(o => o.ShipperID);
        //}
    }
}
