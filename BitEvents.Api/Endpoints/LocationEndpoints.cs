using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Contracts.Requests;
using BitEvents.Api.Domain.Enums;
using FluentValidation;
using BitEvents.Api.Endpoints.Internal;
using BitEvents.Api.Mappers;
using BitEvents.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BitEvents.Api.Endpoints;

public sealed class LocationEndpoints : IEndpoints
{
    private const string BaseRoute = "/locations";

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BaseRoute, CreateLocation)
            .RequireAuthorization(RolesEnum.Admin.ToString());
        // .RequireAuthorization(RolesEnum.Admin.ToString());
        app.MapPut(BaseRoute + "/{id}", UpdateLocation)
            .RequireAuthorization(RolesEnum.Admin.ToString());
        app.MapDelete(BaseRoute + "/{id}", DeleteLocation)
            .RequireAuthorization(RolesEnum.Admin.ToString());
        app.MapGet(BaseRoute, GetAllLocations)
            .AllowAnonymous();
        app.MapGet(BaseRoute + "/{id}", GetLocationById)
            .AllowAnonymous();
    }

    internal static async Task<IResult> CreateLocation(
        [FromBody] LocationCreateRequest locationCreateDto,
        ILocationService locationService,
        IValidator<LocationCreateRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(locationCreateDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var location = await locationService.CreateLocationAsync(locationCreateDto);
        var locationResponse = LocationMapper.LocationToLocationViewResponse(location);
        return Results.Created($"{BaseRoute}/{location.Id}", locationResponse);
    }

    internal static async Task<IResult> UpdateLocation(
        string id,
        [FromBody] LocationUpdateRequest locationUpdateDto,
        ILocationService locationService,
        IValidator<LocationUpdateRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(locationUpdateDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var location = await locationService.UpdateLocationAsync(id, locationUpdateDto);
        return location is null
            ? Results.NotFound("there is not a location with specified id")
            : Results.Ok(LocationMapper.LocationToLocationViewResponse(location));
    }

    internal static async Task<IResult> DeleteLocation(
        string id,
        ILocationService locationService)
    {
        var result = await locationService.DeleteLocationAsync(id);
        return result
            ? Results.Ok("Location successfully deleted")
            : Results.NotFound($"Use with an Id:{id} does not exit");
    }

    internal static async Task<IResult> GetAllLocations(
        [AsParameters] LocationQueryFilter locationQueryFilter,
        ILocationService locationService)
    {
        var locations = await locationService.GetAllLocationsAsync(locationQueryFilter);
        var locationsResponse = LocationMapper.LocationToLocationViewResponseEnumerable(locations);
        return Results.Ok(locationsResponse);
    }

    internal static async Task<IResult> GetLocationById(
        string id,
        ILocationService locationService)
    {
        var location = await locationService.GetLocationByIdAsync(id);
        return location is not null
            ? Results.Ok(LocationMapper.LocationToLocationViewResponse(location))
            : Results.NotFound($"There is not such a location with a given Id: {id}");
    }
}