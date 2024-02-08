namespace BitEvents.Api.Contracts.QueryFilters;

public sealed class OrganizationQueryFilter : QueryFilterBase
{
    public string? Name { get; set; }
    public DateTime? MinCreatedAtUtc { get; set; }
    public DateTime? MaxCreatedAtUtc { get; set; }
}