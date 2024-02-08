using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Contracts.Requests;
using BitEvents.Api.Models;

namespace BitEvents.Api.Services.Interfaces;

public interface IEventService
{
    Task<Event> CreateEventAsync(EventCreateRequest eventCreateRequest, string userId);

    Task<Event?> UpdateEventAsync(string id,
        EventUpdateRequest eventUpdateRequest,
        string userId);

    Task<Event?> GetEventByIdAsync(string id);
    Task<List<Event>> GetEventsAsync(EventQueryFilter eventQueryFilter);
    Task<bool> DeleteEventAsync(string id);
    Task<bool> SponsorEventAsync(string id);
}