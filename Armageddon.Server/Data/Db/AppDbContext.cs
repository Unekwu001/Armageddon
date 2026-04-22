using Armageddon.Server.Data.Interceptors;
using Armageddon.Server.Data.Models;
using Armageddon.Server.Data.Models.AuditAndSoftDeleteModels;
using Armageddon.Server.Data.Models.UserModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Armageddon.Server.Data.Db;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserType> UserTypes { get; set; } 






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

        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
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
