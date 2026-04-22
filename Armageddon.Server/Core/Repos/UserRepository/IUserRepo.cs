using Armageddon.Server.Data.Models.UserModels;

namespace Armageddon.Server.Core.Repos.UserRepository
{
    public interface IUserRepo
    {
        Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search = null,
            bool ascending = true,
            CancellationToken cancellationToken = default);
        Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User?> ValidateUserCredentialsAsync(string email, string passwordHash, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
    }
}
