using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Models;

namespace BitEvents.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync(UserQueryFilter userQueryFilter);
    Task<User?> GetUserByIdAsync(string id, bool includeDeleted = false);
    Task<User?> GetUserWithEmailAsync(string email, bool includeDeleted = false);
    Task<User?> GetUserWithRefreshTokenAsync(string refreshToken);
    Task<bool> CreateUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(string id);
}