﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CommonLib.Model;
using Microsoft.IdentityModel.Tokens;
using TestServiceGRPC.Middleware;
using TestServiceGRPC.Utils.Extensions;

namespace TestServiceGRPC.Utils.Services;

public class TokenService<TSessionData> where TSessionData : class, new()
{
    private readonly RefGuidService _dataReference;

    public TokenService(RefGuidService dataReference) => _dataReference = dataReference;

    //internal static readonly SymmetricSecurityKey TokenKey = new(UTF8.GetBytes(Guid.NewGuid().ToString()));

    // Todo: Remove the hardcoded key and use a key that is generated once when the application starts
    internal static readonly SymmetricSecurityKey TokenKey = new("12345678901234567890123456789012"u8.ToArray());

    public string CreateToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.UniqueName, user.Email),
            new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new(ClaimTypes.Role, user.UserRole.ToString()),

            new(SessionMiddleware<TSessionData>.GuidIdentifier, _dataReference.Value.ToString()!),

            // Add salt to make the token unique
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var creds = new SigningCredentials(TokenKey, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            // Todo: Make the token expiration time configurable
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public AppUser CheckTokenChecksum(HttpContext context, LoginContext loginContext)
    {
        var appUser = context.User.GetAppUser(loginContext);

        if (appUser?.TokenChecksum == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            throw new Exception("Token is invalid!");
        }

        context.Request.Headers.TryGetValue("Authorization", out var bearerValue);

        // Check if the token is in the Authorization header
        // If not, check if the token is in the query string (used for SignalR hubs)
        var bearerString = bearerValue.FirstOrDefault() ?? $"Bearer {context.Request.Query["access_token"]}";

        var checksum = bearerString.GetMd5Checksum();

        if (checksum == appUser.TokenChecksum) return appUser;

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        throw new Exception("Token is invalid!");
    }
}