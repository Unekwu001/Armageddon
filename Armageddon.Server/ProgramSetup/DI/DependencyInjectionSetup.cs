using Armageddon.Server.Core.Repos.UserRepository;
using Armageddon.Server.Data.Db;
using Armageddon.Server.Data.Interceptors;
using Armageddon.Server.ProgramSetup.DbSetup;
using Armageddon.Server.ProgramSetup.Jwt;
using Asp.Versioning;
using System.Text.Json.Serialization;

namespace Armageddon.Server.Data.ProgramSetup.DI
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection SetupDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services.AddEndpointsApiExplorer();

            services.AddSignalR();
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });
            services.AddHttpContextAccessor();
            services.AddSingleton<AuditingAndSoftDeleteInterceptor>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddJwtAuthentication(configuration);
            services.AddOpenApi();
            services.AddApplicationDbContext(configuration);
            services.AddHostedService<EnumSeederHostedService>();
            services.AddAuthorization();
            



            return services;
        }

    }
}
