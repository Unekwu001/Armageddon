using Armageddon.Server.Data.Db;
using Armageddon.Server.Data.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace Armageddon.Server.Core.Repos.UserRepository
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> users, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddRangeAsync(users, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return users;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted, cancellationToken);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == username && !x.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search = null,
            bool ascending = true,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Users.Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Email.Contains(search) ||
                    x.UserName.Contains(search));
            }

            query = ascending
                ? query.OrderBy(x => x.CreatedAt)
                : query.OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null) return false;

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(x => x.Email == email && !x.IsDeleted, cancellationToken);
        }

        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(x => x.UserName == username && !x.IsDeleted, cancellationToken);
        }

        public async Task<User?> ValidateUserCredentialsAsync(string email, string passwordHash, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == email &&
                    x.PasswordHash == passwordHash &&
                    !x.IsDeleted,
                    cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users.CountAsync(x => !x.IsDeleted, cancellationToken);
        }

    }
}
