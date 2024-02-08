namespace BitEvents.Api.Contracts.Responses;

public sealed class CategoryViewResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAtUtc { get; set; }
    public bool IsDeleted { get; set; } = false;
}