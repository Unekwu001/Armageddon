using Armageddon.Server.Data.Db;
using Armageddon.Server.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Armageddon.Server.ProgramSetup.DbSetup
{
    public static class DatabaseServiceExtensions
    {

        public static IServiceCollection AddApplicationDbContext(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // Recommended production settings for PostgreSQL
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);

                    npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);

                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
                    AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
                });

                var auditInterceptor = serviceProvider.GetRequiredService<AuditingAndSoftDeleteInterceptor>();
                options.AddInterceptors(auditInterceptor);

#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
#endif
            });

            return services;
        }
    }
}