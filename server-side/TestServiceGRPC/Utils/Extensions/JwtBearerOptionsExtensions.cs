using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace TestServiceGRPC.Utils.Extensions;

public static class JwtBearerOptionsExtensions
{
    /// <summary>
    /// Configures the JwtBearerOptions for the application
    /// </summary>
    public static void ConfigureJwtBearer(this JwtBearerOptions options, SymmetricSecurityKey tokenKey)
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = tokenKey,
            ValidateIssuer = false,
            ValidateAudience = false
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Read the token out of the query string and set it as the token for SignalR hub requests
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    context.Token = accessToken;

                return Task.CompletedTask;
            }
        };
    }
}