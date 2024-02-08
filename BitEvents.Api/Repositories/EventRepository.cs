using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Models;
using BitEvents.Api.Database;
using BitEvents.Api.Extensions;
using BitEvents.Api.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BitEvents.Api.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IMongoCollection<Event> _eventsCollection;

    public EventRepository(IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);
        _eventsCollection = mongoDatabase.GetCollection<Event>(
            databaseSettings.Value.EventsCollectionName);
    }

    public async Task<bool> CreateEventAsync(Event eventToCreate)
    {
        await _eventsCollection.InsertOneAsync(eventToCreate);
        return true;
    }

    public async Task<bool> UpdateEventAsync(Event eventToUpdate)
    {
        var filter = Builders<Event>.Filter.Eq("Id", eventToUpdate.Id);
        await _eventsCollection.ReplaceOneAsync(filter, eventToUpdate, new ReplaceOptions { IsUpsert = true });
        return true;
    }

    public async Task<Event?> GetEventByIdAsync(string id, bool includeDeleted = false)
    {
        var filter = Builders<Event>.Filter.Eq(x => x.Id, id);
        filter &= Builders<Event>.Filter.Eq(x => x.IsDeleted, includeDeleted);
        return await _eventsCollection
            .Find(filter)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Event>> GetEventsAsync(EventQueryFilter eventQueryFilter)
    {
        var filter = eventQueryFilter.Filter<Event, EventQueryFilter>();
        var sort = QueryExtensions.Sort<Event>(eventQueryFilter);

        return await _eventsCollection
            .Find(filter)
            .Sort(sort)
            .Paginate(eventQueryFilter)
            .ToListAsync();
    }

    public async Task<bool> DeleteEventAsync(string id)
    {
        var filter = Builders<Event>.Filter.Eq("Id", id);
        var update = Builders<Event>.Update
            .Set(x => x.DeletedAtUtc, DateTime.UtcNow)
            .Set(x => x.IsDeleted, true);
        await _eventsCollection.UpdateOneAsync(filter, update);
        return true;
    }
}