using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Boiler> Boilers { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<PickupPoint> PickupPoints { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Boiler>()
                 .HasOne(s => s.Brand)
                 .WithMany(b => b.Boilers)
                 .HasForeignKey(s => s.BrandId);

            modelBuilder.Entity<Boiler>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Boilers)
                .HasForeignKey(s => s.CategoryId);

            modelBuilder.Entity<Client>()
                .HasOne(s => s.Role)
                .WithMany(s => s.Clients)
                .HasForeignKey(s => s.RoleId);

            modelBuilder.Entity<Cart>()
                .HasOne(s => s.Client)
                .WithMany(s => s.Carts)
                .HasForeignKey(s => s.ClientId);

            modelBuilder.Entity<CartItem>()
                .HasOne(s => s.Boiler)
                .WithMany(s => s.CartItems)
                .HasForeignKey(s => s.BoilerId);

            modelBuilder.Entity<CartItem>()
                .HasOne(s => s.Cart)
                .WithMany(s => s.CartItems)
                .HasForeignKey(s => s.CartId);

            modelBuilder.Entity<Order>()
                 .HasOne(s => s.Client)
                 .WithMany(b => b.Orders)
                 .HasForeignKey(s => s.ClientId);

            modelBuilder.Entity<Order>()
                 .HasOne(s => s.PickupPoint)
                 .WithMany(b => b.Orders)
                 .HasForeignKey(s => s.PickupPointId);

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(b => b.Name)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(b => b.Username)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(b => b.Email)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(b => b.Phone)
                .IsUnique();

            modelBuilder.Entity<Brand>()
                .HasIndex(b => b.Name)
                .IsUnique();

            modelBuilder.Entity<PickupPoint>()
                .HasIndex(b => b.City)
                .IsUnique();

            modelBuilder.Entity<PickupPoint>()
                .HasIndex(b => b.Address)
                .IsUnique();
        }
    }
}
