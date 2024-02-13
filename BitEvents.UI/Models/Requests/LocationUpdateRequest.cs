namespace BitEvents.UI.Models.Requests;

public sealed class LocationUpdateRequest
{
    public required string Name { get; set; }
    public string? AccommodationPartnerId { get; set; }
    public string? TransportPartnerId { get; set; }
}