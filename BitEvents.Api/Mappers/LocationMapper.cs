using BitEvents.Api.Contracts.Requests;
using BitEvents.Api.Contracts.Responses;
using BitEvents.Api.Models;
using Riok.Mapperly.Abstractions;

namespace BitEvents.Api.Mappers;

[Mapper]
public static partial class LocationMapper
{
    public static partial Location LocationCreateRequestToLocation(LocationCreateRequest locationCreateRequest);
    public static partial LocationViewResponse LocationToLocationViewResponse(Location location);
    public static partial LocationPartial LocationToLocationPartial(Location location);

    public static partial IEnumerable<LocationViewResponse> LocationToLocationViewResponseEnumerable(
        IEnumerable<Location> location);
}