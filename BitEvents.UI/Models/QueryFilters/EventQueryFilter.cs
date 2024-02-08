namespace BitEvents.Api.Contracts.QueryFilters;

public sealed class EventQueryFilter : QueryFilterBase
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? MinStartingDate { get; set; }
    public DateTime? MaxStartingDate { get; set; }
    public DateTime? MinEndingDate { get; set; }
    public DateTime? MaxEndingDate { get; set; }
    public string? GuestsList { get; set; }
    public string? CompetitorsList { get; set; }
    public ulong? MinCapacity { get; set; }
    public ulong? MaxCapacity { get; set; }
    public double? MinTicketPrice { get; set; }
    public double? MaxTicketPrice { get; set; }
    public string? Street { get; set; }
    public string? LocationId { get; set; }
    public string? OrganizationId { get; set; }
    public string? CategoryId { get; set; }
    public uint? MinViews { get; set; }
    public uint? MaxViews { get; set; }
    public uint? MinGoing { get; set; }
    public uint? MaxGoing { get; set; }
    public uint? MinFavourites { get; set; }
    public uint? MaxFavourites { get; set; }
}