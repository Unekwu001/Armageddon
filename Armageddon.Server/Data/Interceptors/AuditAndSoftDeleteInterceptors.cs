using Armageddon.Server.Data.Models.AuditAndSoftDeleteModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;

namespace Armageddon.Server.Data.Interceptors
{
    public sealed class AuditingAndSoftDeleteInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditingAndSoftDeleteInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            UpdateAuditableEntities(eventData.Context);
            HandleSoftDelete(eventData.Context);

            return base.SavingChanges(eventData, result);
        }



        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateAuditableEntities(eventData.Context);
            HandleSoftDelete(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }


        private void UpdateAuditableEntities(DbContext? context)
        {
            if (context == null) return;

            var now = DateTime.UtcNow;
            var currentUser = GetCurrentUserIdOrName();

            foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = currentUser;

                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedBy = currentUser;

                        entry.Entity.LastUpdatedAt = now;
                        entry.Entity.LastUpdatedBy = currentUser;

                        entry.Entity.IsActive = true;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedBy = currentUser;

                        entry.Entity.LastUpdatedAt = now;
                        entry.Entity.LastUpdatedBy = currentUser;
                        break;
                }
            }
        }


        private void HandleSoftDelete(DbContext? context)
        {
            if (context == null) return;

            var now = DateTime.UtcNow;
            var currentUser = GetCurrentUserIdOrName();

            foreach (var entry in context.ChangeTracker.Entries<ISoftDeletableEntity>()
                .Where(e => e.State == EntityState.Deleted))
            {
                entry.State = EntityState.Modified;

                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = now;
                entry.Entity.DeletedBy = currentUser;

                entry.Entity.IsActive = false;

                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = currentUser;

                entry.Entity.LastUpdatedAt = now;
                entry.Entity.LastUpdatedBy = currentUser;
            }
        }




        private string? GetCurrentUserIdOrName()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return "Unknown";

            return httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                ?? httpContext.User.Identity.Name
                ?? "Unknown";
        }




    }




    
}
