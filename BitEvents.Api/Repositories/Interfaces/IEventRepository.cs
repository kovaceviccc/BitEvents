using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Models;

namespace BitEvents.Api.Repositories.Interfaces;

public interface IEventRepository
{
    Task<bool> CreateEventAsync(Event eventToCreate);
    Task<bool> UpdateEventAsync(Event eventToUpdate);
    Task<Event?> GetEventByIdAsync(string id, bool includeDeleted = false);
    Task<List<Event>> GetEventsAsync(EventQueryFilter eventQueryFilter);
    Task<bool> DeleteEventAsync(string id);
}