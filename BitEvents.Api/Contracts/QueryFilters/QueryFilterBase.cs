namespace BitEvents.Api.Contracts.QueryFilters;

public class QueryFilterBase
{
    public int? PageSize { get; set; } = 10;
    public int? PageNumber { get; set; } = 1;
    public string? SortColumn { get; set; }
    public string? SortDirection { get; set; } = "";
    public string? IntersectionColumns { get; set; }
    public string? UnionColumns { get; set; }
    public bool? ShowDeleted { get; set; } = false;
}