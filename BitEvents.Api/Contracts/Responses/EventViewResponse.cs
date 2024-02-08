using BitEvents.Api.Models;

namespace BitEvents.Api.Contracts.Responses;

public class EventViewResponse
{
    public required string Id { get; set; }
    public required DateTime CreatedAtUtc { get; set; }
    public required DateTime UpdatedAtUtc { get; set; }
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
    public bool Sponsored { get; set; } = false;
    public required LocationPartial Location { get; set; }
    public required string Street { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public uint Views { get; set; } = 0;
    public uint Going { get; set; } = 0;
    public uint Favourites { get; set; } = 0;
}