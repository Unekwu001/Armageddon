using Armageddon.Server.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Armageddon.Server.ProgramSetup.DbSetup
{
    public static class DataBaseAutoMigrationSetup
    {
        public static void ApplyDatabaseMigrations(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
