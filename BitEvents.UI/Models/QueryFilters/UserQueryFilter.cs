namespace BitEvents.Api.Contracts.QueryFilters;

public sealed class UserQueryFilter : QueryFilterBase
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateTime? MinCreatedAtUtc { get; set; }
    public DateTime? MaxCreatedAtUtc { get; set; }
}