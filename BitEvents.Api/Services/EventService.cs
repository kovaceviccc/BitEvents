using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Contracts.Requests;
using BitEvents.Api.Models;
using BitEvents.Api.Exceptions;
using BitEvents.Api.Mappers;
using BitEvents.Api.Repositories.Interfaces;
using BitEvents.Api.Services.Interfaces;

namespace BitEvents.Api.Services;

public class EventService : IEventService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ILocationRepository _locationRepository;

    public EventService(IEventRepository eventRepository,
        ICategoryRepository categoryRepository,
        ILocationRepository locationRepository)
    {
        _eventRepository = eventRepository;
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<Event> CreateEventAsync(EventCreateRequest eventCreateRequest, string userId)
    {
        var eventToCreate = EventMapper.EventCreateRequestToEvent(eventCreateRequest);

        eventToCreate.Category = CategoryMapper.CategoryToCategoryPartial(
            await _categoryRepository.GetCategoryByIdAsync(eventCreateRequest.CategoryId) ??
            throw new NotFoundException(
                $"There is no category with the id of: {eventCreateRequest.CategoryId}"));
        
        var location = await _locationRepository.GetLocationByIdAsync(eventCreateRequest.LocationId);
        eventToCreate.Location = LocationMapper.LocationToLocationPartial(
            location ??
            throw new NotFoundException(
                $"There is no location with the id of: {eventCreateRequest.LocationId}"));
        
        await _eventRepository.CreateEventAsync(eventToCreate);
        return eventToCreate;
    }

    public async Task<Event?> UpdateEventAsync(string id,
        EventUpdateRequest eventUpdateRequest,
        string userId)
    {
        var eventToUpdate = await _eventRepository.GetEventByIdAsync(id);
        
        if (eventToUpdate is null)
        {
            return null;
        }

        eventToUpdate.Title = eventUpdateRequest.Title;
        eventToUpdate.Description = eventUpdateRequest.Description;
        eventToUpdate.StartingDate = eventUpdateRequest.StartingDate;
        eventToUpdate.EndingDate = eventUpdateRequest.EndingDate;
        eventToUpdate.ImageUrls = eventUpdateRequest.ImageUrls;
        eventToUpdate.Guests = eventUpdateRequest.Guests;
        eventToUpdate.Competitors = eventUpdateRequest.Competitors;
        eventToUpdate.Capacity = eventUpdateRequest.Capacity;
        eventToUpdate.TicketPrice = eventUpdateRequest.TicketPrice;
        eventToUpdate.TicketUrl = eventUpdateRequest.TicketUrl;
        eventToUpdate.Sponsors = eventUpdateRequest.Sponsors;
        eventToUpdate.Street = eventUpdateRequest.Street;
        eventToUpdate.Latitude = eventUpdateRequest.Latitude;
        eventToUpdate.Longitude = eventUpdateRequest.Longitude;
        eventToUpdate.UpdatedAtUtc = DateTime.UtcNow;

        await _eventRepository.UpdateEventAsync(eventToUpdate);
        return eventToUpdate;
    }

    public async Task<Event?> GetEventByIdAsync(string id)
    {
        var eventToGet = await _eventRepository.GetEventByIdAsync(id);

        if (eventToGet is null)
        {
            return null;
        }

        eventToGet.Views++;
        await _eventRepository.UpdateEventAsync(eventToGet);

        return eventToGet;
    }

    public async Task<List<Event>> GetEventsAsync(EventQueryFilter eventQueryFilter)
    {
        return await _eventRepository.GetEventsAsync(eventQueryFilter);
    }

    public async Task<bool> DeleteEventAsync(string id)
    {
        var eventToDelete = await _eventRepository.GetEventByIdAsync(id);

        if (eventToDelete is null)
        {
            return false;
        }

        return await _eventRepository.DeleteEventAsync(id);
    }

    public async Task<bool> SponsorEventAsync(string id)
    {
        var eventToUpdate = await _eventRepository.GetEventByIdAsync(id);

        if (eventToUpdate is null)
        {
            return false;
        }

        if (eventToUpdate.Sponsored)
        {
            return true;
        }

        eventToUpdate.Sponsored = true;
        return await _eventRepository.UpdateEventAsync(eventToUpdate);
    }
}