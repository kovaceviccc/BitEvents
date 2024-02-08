using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Contracts.Requests;
using BitEvents.Api.Models;
using BitEvents.Api.Exceptions;
using BitEvents.Api.Mappers;
using BitEvents.Api.Repositories.Interfaces;
using BitEvents.Api.Services.Interfaces;

namespace BitEvents.Api.Services;

public sealed class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<List<Location>> GetAllLocationsAsync(LocationQueryFilter locationQueryFilter)
    {
        return await _locationRepository.GetAllLocationsAsync(locationQueryFilter);
    }

    public async Task<Location?> GetLocationByIdAsync(string id)
    {
        return await _locationRepository.GetLocationByIdAsync(id);
    }

    public async Task<Location> CreateLocationAsync(LocationCreateRequest locationCreateRequest)
    {
        var existingLocation = await _locationRepository.GetLocationByNameAsync(locationCreateRequest.Name);
        if (existingLocation is not null)
        {
            throw new InvalidInputException("Location with a given name already exists");
        }

        var location = LocationMapper.LocationCreateRequestToLocation(locationCreateRequest);
        
        var success = await _locationRepository.CreateLocationAsync(location);
        if (!success)
        {
            throw new DatabaseException("Failed to create location");
        }

        return location;
    }

    public async Task<Location?> UpdateLocationAsync(string id, LocationUpdateRequest locationUpdateRequest)
    {
        var existingLocation = await _locationRepository.GetLocationByIdAsync(id);
        if (existingLocation is null)
        {
            return null;
        }

        existingLocation.Name = locationUpdateRequest.Name;
        existingLocation.UpdatedAtUtc = DateTime.Now;

        var success = await _locationRepository.UpdateLocationAsync(existingLocation);
        if (!success)
        {
            throw new DatabaseException("Failed to update Location");
        }

        return existingLocation;
    }

    public async Task<bool> DeleteLocationAsync(string id)
    {
        var existingLocation = await _locationRepository.GetLocationByIdAsync(id);
        if (existingLocation is null)
        {
            return false;
        }

        existingLocation.IsDeleted = true;
        existingLocation.DeletedAtUtc = DateTime.Now;
        return await _locationRepository.UpdateLocationAsync(existingLocation);
    }
}