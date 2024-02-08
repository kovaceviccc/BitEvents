namespace BitEvents.Api.Contracts.Requests;

public sealed class EventUpdateRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime StartingDate { get; set; }
    public required DateTime EndingDate { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Guests { get; set; } = new();
    public List<string> Competitors { get; set; } = new();
    public required ulong Capacity { get; set; }
    public double? TicketPrice { get; set; }
    public string? TicketUrl { get; set; }
    public List<string> Sponsors { get; set; } = new();
    public required string Street { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
}