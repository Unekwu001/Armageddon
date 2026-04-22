using Armageddon.Server.Data.ProgramSetup.DI;
using Armageddon.Server.ProgramSetup.DbSetup;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Logging.AddFilter("EnumSeederHostedService", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Services.SetupDependencyInjection(builder.Configuration);

var app = builder.Build();

app.ApplyDatabaseMigrations();
app.MapDefaultEndpoints();
app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Armageddon - Backend Service")
            .WithTheme(ScalarTheme.DeepSpace)        // Try: Purple, Mars, Moon, etc.
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
