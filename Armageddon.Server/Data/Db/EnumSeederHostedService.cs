using Armageddon.Server.Data.Enums;
using Armageddon.Server.Data.Models;
using Armageddon.Server.Data.Models.OrderModels;
using Armageddon.Server.Data.Models.PaymentModels;
using Armageddon.Server.Data.Models.ProductModels;
using Armageddon.Server.Data.Models.UserModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

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
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var strategy = db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Applying pending migrations...");
                await db.Database.MigrateAsync(cancellationToken);

                _logger.LogInformation("Starting enum seeding...");
                await using var transaction =
                    await db.Database.BeginTransactionAsync(cancellationToken);

                int totalAdded = 0;

                totalAdded += await SeedTable<UserTypeEnum, UserType>(db, e => new UserType { Id = (int)e, Name = e.GetDisplayName(), Code = e.ToString().ToLowerInvariant() }, cancellationToken);
                totalAdded += await SeedTable<PaymentStatusEnum, PaymentStatus>(db, e => new PaymentStatus { Id = (int)e, Name = e.GetDisplayName(), Code = e.ToString().ToLowerInvariant() }, cancellationToken);
                totalAdded += await SeedTable<PaymentTypeEnum, PaymentType>(db, e => new PaymentType { Id = (int)e, Name = e.GetDisplayName(), Code = e.ToString().ToLowerInvariant() }, cancellationToken);
                totalAdded += await SeedTable<DeliveryStatusEnum, DeliveryStatus>(db, e => new DeliveryStatus { Id = (int)e, Name = e.GetDisplayName(), Code = e.ToString().ToLowerInvariant() }, cancellationToken);
                totalAdded += await SeedTable<ProductTypeEnum, ProductType>(db, e => new ProductType { Id = (int)e, Name = e.GetDisplayName(), Code = e.ToString().ToLowerInvariant() }, cancellationToken);

                await db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Enum seeding completed. Added {Count} records", totalAdded);
            });
            stopwatch.Stop();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Enum seeding failed");
            throw;
        }
    }





    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;





    private async Task<int> SeedTable<TEnum, TEntity>(
            AppDbContext db,
            Func<TEnum, TEntity> entityFactory,
            CancellationToken ct)
            where TEnum : struct, Enum
            where TEntity : class, IHasIntegerId
    {
        var entityTypeName = typeof(TEntity).Name;
        var set = db.Set<TEntity>();
        var existingIds = await set.Select(x => x.Id).ToListAsync();

        var existingIdsSet = new HashSet<int>(existingIds);

        var toAdd = new List<TEntity>();

        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            var id = Convert.ToInt32(enumValue);

            if (existingIdsSet.Contains(id))
                continue;

            var entity = entityFactory(enumValue);
            toAdd.Add(entity);
        }

        if (toAdd.Count > 0)
        {
            await set.AddRangeAsync(toAdd, ct);
            _logger.LogInformation("Preparing to add {Count} new {EntityType} records", toAdd.Count, entityTypeName);
        }
        else
        {
            _logger.LogDebug("No new {EntityType} records to seed", entityTypeName);
        }

        return toAdd.Count;
    }


}