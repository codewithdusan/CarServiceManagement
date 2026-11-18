
using Microsoft.EntityFrameworkCore;
using CarServiceManagement.Models;

namespace CarServiceManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ServiceOrder>()
                .Property(nalog => nalog.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ServiceOrder>()
                .HasOne(nalog => nalog.Customer)
                .WithMany()
                .HasForeignKey(nalog => nalog.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ServiceOrder>()
                .HasOne(nalog => nalog.Vehicle)
                .WithMany(vozilo => vozilo.ServiceOrders)
                .HasForeignKey(nalog => nalog.VehicleId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
