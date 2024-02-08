namespace BitEvents.Api.Contracts.Requests;

public sealed class LocationCreateRequest
{
    public required string Name { get; set; }
    public string? AccommodationPartnerId { get; set; }
    public string? TransportPartnerId { get; set; }
}