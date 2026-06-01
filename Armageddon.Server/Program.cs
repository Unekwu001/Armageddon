using Armageddon.Server.Common.ProgramSetup.DbSetup;
using Armageddon.Server.Common.ProgramSetup.DI;
using Armageddon.Server.Core.Hubs;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Logging.AddFilter("EnumSeederHostedService", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Services.SetupDependencyInjection(builder.Configuration);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Middleware
app.ApplyDatabaseMigrations();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

// Development tools
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Armageddon - Backend Service")
               .WithTheme(ScalarTheme.DeepSpace)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
               .EnableDarkMode()
               .AddPreferredSecuritySchemes("Bearer")
               .AddHttpAuthentication("Bearer", _ => { });
    });
}

// Static files
app.UseDefaultFiles();
app.MapStaticAssets();

// Map Endpoints
app.MapDefaultEndpoints();
app.MapControllers();
app.MapFallbackToFile("/index.html");

// SignalR Hub - MUST be after UseRouting/UseEndpoints setup
app.MapHub<SellerHub>("/sellerHub");

app.Run();