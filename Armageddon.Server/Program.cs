using Armageddon.Server.Common.ProgramSetup.DbSetup;
using Armageddon.Server.Common.ProgramSetup.DI;
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
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .EnableDarkMode()
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication("Bearer", _ => { });
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
