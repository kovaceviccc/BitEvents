namespace BitEvents.Api.Contracts.QueryFilters;

public sealed class CategoryQueryFilter : QueryFilterBase
{
    public string? Name { get; set; }
    public DateTime? MinCreatedAtUtc { get; set; }
    public DateTime? MaxCreatedAtUtc { get; set; }
}