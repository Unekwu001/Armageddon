using Armageddon.Server.Common.ProgramSetup.Cors;
using Armageddon.Server.Common.ProgramSetup.DbSetup;
using Armageddon.Server.Common.ProgramSetup.Jwt;
using Armageddon.Server.Core.Repos.OrderRepository;
using Armageddon.Server.Core.Repos.ProductRepository;
using Armageddon.Server.Core.Repos.UserRepository;
using Armageddon.Server.Core.Services;
using Armageddon.Server.Data.Db;
using Armageddon.Server.Data.Interceptors;
using Asp.Versioning;
using System.Text.Json.Serialization;

namespace Armageddon.Server.Common.ProgramSetup.DI
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
            services.AddCustomCors();
            services.AddSingleton<AuditingAndSoftDeleteInterceptor>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUserService, UserServices>();
            services.AddScoped<IProductRepo, ProductRepo>();
            services.AddScoped<IProductServices, ProductServices>();  
            services.AddScoped<IOrderRepo, OrderRepo>();
            services.AddScoped<IOrderService, OrderServices>();
            services.AddJwtAuthentication(configuration);
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });
            services.AddApplicationDbContext(configuration);
            services.AddHostedService<EnumSeederHostedService>();
            services.AddAuthorization();
            



            return services;
        }

    }
}
