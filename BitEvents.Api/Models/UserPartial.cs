namespace BitEvents.Api.Models;

public class UserPartial
{
    public required string Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}