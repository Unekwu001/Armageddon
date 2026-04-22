using Armageddon.Server.Core.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Armageddon.Server.ProgramSetup.Jwt
{
    public static class JwtSetup
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Load JWT settings from config
            var jwtSection = configuration.GetSection("Jwt");
            var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

            services.Configure<JwtSettings>(jwtSection);
            services.AddSingleton(jwtSettings);

            // Register authentication + JWT bearer scheme
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

            });

            return services;
        }
    }
}
