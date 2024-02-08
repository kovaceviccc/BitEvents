using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Contracts.Requests;
using BitEvents.Api.Domain.Enums;
using FluentValidation;
using BitEvents.Api.Endpoints.Internal;
using BitEvents.Api.Extensions;
using BitEvents.Api.Mappers;
using BitEvents.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BitEvents.Api.Endpoints;

public class EventEndpoints : IEndpoints
{
    private const string BaseRoute = "/events";

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BaseRoute, CreateEvent)
            .RequireAuthorization(RolesEnum.User.ToString());

        app.MapGet(BaseRoute, GetAllEvents)
            .AllowAnonymous();

        app.MapGet(BaseRoute + "/{id}", GetEventById)
            .AllowAnonymous();

        app.MapPut(BaseRoute + "/{id}", UpdateEvent)
            .RequireAuthorization(RolesEnum.User.ToString());

        app.MapDelete(BaseRoute + "/{id}", DeleteEvent)
            .RequireAuthorization(RolesEnum.User.ToString());

        app.MapPatch(BaseRoute + "/{id}/sponsor", SponsorEvent)
            .RequireAuthorization(RolesEnum.Admin.ToString());
    }

    internal static async Task<IResult> CreateEvent(
        [FromBody] EventCreateRequest createEventRequest,
        IValidator<EventCreateRequest> validator,
        IEventService eventService,
        HttpContext httpContext)
    {
        var validationResult = await validator.ValidateAsync(createEventRequest);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var createdEvent = await eventService.CreateEventAsync(createEventRequest, httpContext.User.GetUserId());
        return Results.Created($"{BaseRoute}/{createdEvent.Id}",
            EventMapper.EventToEventViewResponse(createdEvent));
    }

    internal static async Task<IResult> GetAllEvents(
        [AsParameters] EventQueryFilter eventQueryFilter,
        IEventService eventService)
    {
        var events = await eventService.GetEventsAsync(eventQueryFilter);
        return Results.Ok(EventMapper.EventListToEventViewResponseList(events));
    }

    internal static async Task<IResult> GetEventById(
        string id,
        IEventService eventService)
    {
        var eventModel = await eventService.GetEventByIdAsync(id);
        return eventModel is null
            ? Results.NotFound($"Event with the id of: {id} does not exist")
            : Results.Ok(EventMapper.EventToEventViewResponse(eventModel));
    }

    internal static async Task<IResult> UpdateEvent(
        string id,
        [FromBody] EventUpdateRequest updateEventRequest,
        IValidator<EventUpdateRequest> validator,
        IEventService eventService,
        HttpContext httpContext)
    {
        var validationResult = await validator.ValidateAsync(updateEventRequest);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var updatedEvent = await eventService.UpdateEventAsync(id, updateEventRequest, httpContext.User.GetUserId());
        return updatedEvent is null
            ? Results.NotFound($"Event with the id of: {id} does not exist")
            : Results.Ok(EventMapper.EventToEventViewResponse(updatedEvent));
    }

    internal static async Task<IResult> DeleteEvent(
        string id,
        IEventService eventService)
    {
        var deleted = await eventService.DeleteEventAsync(id);
        return !deleted
            ? Results.NotFound($"Event with the id of: {id} does not exist")
            : Results.Ok($"Event with the id of: {id} has been deleted");
    }

    internal static async Task<IResult> SponsorEvent(
        string id,
        IEventService eventService)
    {
        var success = await eventService.SponsorEventAsync(id);
        return !success
            ? Results.NotFound($"Event with the id of: {id} does not exist")
            : Results.Ok($"Event with the id of: {id} is sponsored");
    }
}