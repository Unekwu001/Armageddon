using Armageddon.Server.Data.Interceptors;
using Armageddon.Server.Data.Models;
using Armageddon.Server.Data.Models.AuditAndSoftDeleteModels;
using Armageddon.Server.Data.Models.OrderModels;
using Armageddon.Server.Data.Models.PaymentModels;
using Armageddon.Server.Data.Models.ProductModels;
using Armageddon.Server.Data.Models.UserModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Armageddon.Server.Data.Db;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserType> UserTypes { get; set; }
    public DbSet<PaymentStatus> PaymentStatuses { get; set; }
    public DbSet<PaymentType> PaymentTypes { get; set; }
    public DbSet<DeliveryStatus> DeliveryStatuses { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; } 


    private readonly AuditingAndSoftDeleteInterceptor _auditingInterceptor;
    public AppDbContext(DbContextOptions<AppDbContext> options, AuditingAndSoftDeleteInterceptor auditingInterceptor)
        : base(options)
    {
        _auditingInterceptor = auditingInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditingInterceptor);
    }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(entity => entity.UserCode).IsRequired().HasMaxLength(10).ValueGeneratedOnAdd();
            entity.Property(e => e.UserTypeId).IsRequired();  
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.WalletAddress).HasMaxLength(200);
            entity.HasQueryFilter(u => !u.IsDeleted);

            entity.HasIndex(x => x.UserCode).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.UserName).IsUnique();
            entity.HasIndex(e => e.PhoneNumber).IsUnique();
            entity.HasIndex(e => e.WalletAddress).IsUnique();


        });


        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.PricePerGram).IsRequired();
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.ProductTypeId).IsRequired();
            entity.Property(e => e.Stock).IsRequired();
            entity.Property(e => e.SellerId).IsRequired();
            entity.Property(e => e.Stock).IsRequired();
            entity.HasQueryFilter(u => !u.IsDeleted);
        });


        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.BuyerId).IsRequired();
            entity.Property(e => e.CryptoTransactionId).HasMaxLength(255);
            entity.Property(e => e.DeliveryStatusId).IsRequired();
            entity.Property(e => e.TotalAmount).IsRequired();
            entity.Property(e => e.PaymentStatusId).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired();
            entity.HasQueryFilter(u => !u.IsDeleted);

            entity.HasMany(e => e.Items)
            .WithOne(c => c.Order)
            .HasForeignKey(c => c.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.OrderId).IsRequired();
            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.UnitPrice).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired();
            entity.HasQueryFilter(u => !u.IsDeleted);
        });


        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletableEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");

            var isDeletedProperty = Expression.Property(
                parameter,
                nameof(Base.IsDeleted)
            );

            var filter = Expression.Lambda(
                Expression.Equal(isDeletedProperty, Expression.Constant(false)),
                parameter
            );

            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(filter);
        }





    }
}
