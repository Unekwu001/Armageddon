namespace Armageddon.Server.Data.ProgramSetup.Cors
{
    public static class CorsServiceSetup
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            var allowedOrigins = new[]
            {
                    "http://localhost:5500",
                    "http://127.0.0.1:5500",
                    "http://localhost:3000",
                    "http://localhost:8000",
                    "http://localhost:5173",
                    "http://localhost:52934"
            };
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            return services;
        }
    }
}
