using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Models;

namespace BitEvents.Api.Repositories.Interfaces;

public interface ILocationRepository
{
    Task<List<Location>> GetAllLocationsAsync(LocationQueryFilter locationQueryFilter);
    Task<Location?> GetLocationByIdAsync(string id, bool includeDeleted = false);
    Task<Location?> GetLocationByNameAsync(string name, bool includeDeleted = false);
    Task<bool> CreateLocationAsync(Location location);
    Task<bool> UpdateLocationAsync(Location location);
    Task<bool> DeleteLocationAsync(string id);
}