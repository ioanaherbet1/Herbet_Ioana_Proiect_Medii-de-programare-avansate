using Herbet_Ioana_Proiect.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace Herbet_Ioana_Proiect.Data
{
    public class DressesContext : DbContext
    {
        public DressesContext(DbContextOptions<DressesContext> options) :
       base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Dress> Dresses { get; set; }
        public DbSet<Designer> Designers { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<AvailableDress> AvailableDresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<Dress>().ToTable("Dress");
            modelBuilder.Entity<Designer>().ToTable("Designer");
            modelBuilder.Entity<Shop>().ToTable("Shop");
            modelBuilder.Entity<AvailableDress>().ToTable("AvailableDress");
            
            modelBuilder.Entity<AvailableDress>()
            .HasKey(c => new { c.DressID, c.ShopID });
        }


    }

}
