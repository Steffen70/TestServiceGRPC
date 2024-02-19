using System.Security.Claims;
using CommonLib.Model;
using Microsoft.EntityFrameworkCore;
using TestServiceGRPC.Middleware;
using TestServiceGRPC.Model;

namespace TestServiceGRPC.Utils.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
        => Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Can't get UserId from ClaimsPrincipal"));

    public static AppUser GetAppUser(this ClaimsPrincipal user, LoginContext context)
        => context.Users
            .Include(u => u.SessionTokens)
            .FirstOrDefault(u => u.Id == user.GetUserId()) 
           ?? throw new Exception("Can't get User from ClaimsPrincipal");

    public static Guid? GetDataReference(this ClaimsPrincipal user)
    {
        // Get the Guid from the ClaimsPrincipal that is stored in the JWT token
        var guidstr = user.FindFirst(SessionMiddleware<object>.GuidIdentifier)?.Value;

        return Guid.TryParse(guidstr, out var refGuid) ? refGuid : null;
    }
}