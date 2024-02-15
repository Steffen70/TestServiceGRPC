using CommonLib.Model;
using Microsoft.AspNetCore.Authorization;

namespace TestServiceGRPC.Utils.Extensions;

public static class AuthorizationOptionsExtensions
{
    /// <summary>
    /// Adds a policy for each AppUserRole.
    /// </summary>
    public static void AddAppUserRoles(this AuthorizationOptions options)
    {
        // loop through all AppUserRoles and add a policy for each role
        foreach (var role in Enum.GetValues<AppUserRole>())
            options.AddPolicy(role.ToString(), policy => policy.RequireRole(role.ToString()));
    }
}
