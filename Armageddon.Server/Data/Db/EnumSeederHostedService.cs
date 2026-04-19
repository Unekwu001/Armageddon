using Armageddon.Server.Data.Enums;
using Armageddon.Server.Data.Models;
using Armageddon.Server.Data.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace Armageddon.Server.Data.Db;
public class EnumSeederHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EnumSeederHostedService> _logger;

    public EnumSeederHostedService(
        IServiceProvider serviceProvider,
        ILogger<EnumSeederHostedService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }





    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            _logger.LogInformation("Applying pending migrations...");
            await db.Database.MigrateAsync(cancellationToken);

            _logger.LogInformation("Starting enum seeding...");
            await SeedEnums(db, cancellationToken);

            await db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Enum seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Enum seeding failed during startup.");
            throw; 
        }
    }



    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;





    private string GetDisplayNameForUserTypeEnum(UserTypeEnum value) => value switch
    {
        UserTypeEnum.Buyer => "Buyer",
        UserTypeEnum.Seller => "Seller",
        UserTypeEnum.Admin => "Admin",
        UserTypeEnum.SuperAdmin => "Super Admin",

        _ => value.ToString()
    };




    private async Task SeedEnums(AppDbContext db, CancellationToken ct = default)
    {


        await SeedTable<UserTypeEnum, UserType>(db, ct,
          e => new UserType
          {
              Id = (int)e,
              Name = GetDisplayNameForUserTypeEnum(e),
              Code = e.ToString().ToLowerInvariant()
          });
    }





    private async Task SeedTable<TEnum, TEntity>(
        AppDbContext db,
        CancellationToken ct,
        Func<TEnum, TEntity> factory)
        where TEnum : struct, Enum
        where TEntity : class, IHasIntegerId
    {
        var entityTypeName = typeof(TEntity).Name;
        var set = db.Set<TEntity>();

        _logger.LogInformation("Seeding {EntityType}", entityTypeName);

        foreach (var enumValue in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
        {
            var id = Convert.ToInt32(enumValue);

            if (await set.AnyAsync(x => x.Id == id, ct))
            {
                _logger.LogDebug("Already exists: {EntityType} ID {Id}", entityTypeName, id);
                continue;
            }

            set.Add(factory(enumValue));
            _logger.LogInformation("Added: {EntityType} ID {Id}", entityTypeName, id);
        }

        await db.SaveChangesAsync(ct);
        _logger.LogInformation("Seeding of {EntityType} completed", entityTypeName);
    }




   

}
