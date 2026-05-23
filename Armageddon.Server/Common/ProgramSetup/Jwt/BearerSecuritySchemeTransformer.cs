using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Armageddon.Server.Common.ProgramSetup.Jwt
{

    public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            document.Components ??= new OpenApiComponents();

            document.Components.SecuritySchemes ??=
                new Dictionary<string, IOpenApiSecurityScheme>();

            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT token"
            };

            document.Components.SecuritySchemes["Bearer"] = bearerScheme;

            return Task.CompletedTask;
        }
    }
}
