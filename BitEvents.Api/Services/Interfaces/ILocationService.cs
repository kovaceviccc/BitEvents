using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Contracts.Requests;
using BitEvents.Api.Models;

namespace BitEvents.Api.Services.Interfaces;

public interface ILocationService
{
    Task<List<Location>> GetAllLocationsAsync(LocationQueryFilter locationQueryFilter);
    Task<Location?> GetLocationByIdAsync(string id);
    Task<Location> CreateLocationAsync(LocationCreateRequest locationCreateRequest);
    Task<Location?> UpdateLocationAsync(string id, LocationUpdateRequest locationUpdateRequest);
    Task<bool> DeleteLocationAsync(string id);
}