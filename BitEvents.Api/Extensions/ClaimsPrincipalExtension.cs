using System.Security.Claims;

namespace BitEvents.Api.Extensions;

public static class ClaimsPrincipalExtension
{
    internal static string GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.Claims.FirstOrDefault(c => c.Type == "Id");
        return userId == null ? "" : userId.Value;
    }

    internal static string GetOrganizationId(this ClaimsPrincipal user)
    {
        var organizationId = user.Claims.FirstOrDefault(c => c.Type == "OrganizationId");
        return organizationId == null ? "" : organizationId.Value;
    }

    internal static string GetRole(this ClaimsPrincipal user)
    {
        var userRole = user.Claims.FirstOrDefault(c => c.Type == "Roles");
        return userRole == null ? string.Empty : userRole.Value;
    }
}