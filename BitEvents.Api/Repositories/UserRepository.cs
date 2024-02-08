using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Models;
using BitEvents.Api.Database;
using BitEvents.Api.Extensions;
using BitEvents.Api.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BitEvents.Api.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _usersCollection;

    public UserRepository(IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>(
            databaseSettings.Value.UsersCollectionName);
    }

    public async Task<List<User>> GetAllUsersAsync(UserQueryFilter userQueryFilter)
    {
        var filter = userQueryFilter.Filter<User, UserQueryFilter>();
        var sort = QueryExtensions.Sort<User>(userQueryFilter);

        return await _usersCollection
            .Find(filter)
            .Sort(sort)
            .Paginate(userQueryFilter)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(string id, bool includeDeleted = false)
    {
        var filter = Builders<User>.Filter.Eq(user => user.Id, id);
        filter &= Builders<User>.Filter.Eq(user => user.IsDeleted, includeDeleted);

        return await _usersCollection
            .Find(filter)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserWithEmailAsync(string email, bool includeDeleted = false)
    {
        var filter = Builders<User>.Filter.Eq(user => user.Email, email);
        filter &= Builders<User>.Filter.Eq(user => user.IsDeleted, includeDeleted);

        return await _usersCollection
            .Find(filter)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserWithRefreshTokenAsync(string refreshToken)
    {
        var filter = Builders<User>.Filter.ElemMatch(user => user.RefreshTokens,
            x => x.Token == refreshToken);

        filter &= Builders<User>.Filter.Eq(user => user.IsDeleted, false);

        return await _usersCollection
            .Find(filter)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        await _usersCollection.InsertOneAsync(user);
        return true;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var filter = Builders<User>.Filter.Eq("Id", user.Id);
        await _usersCollection.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
        return true;
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var filter = Builders<User>.Filter.Eq("Id", id);
        var update = Builders<User>.Update
            .Set(x => x.DeletedAtUtc, DateTime.UtcNow)
            .Set(x => x.IsDeleted, true);
        await _usersCollection.UpdateOneAsync(filter, update);
        return true;
    }
}