using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using TestServiceGRPC.Utils;
using TestServiceGRPC.Utils.Extensions;
using TestServiceGRPC.Utils.Services;

namespace TestServiceGRPC.Middleware;

public class SessionMiddleware<TSessionData> where TSessionData : class, new()
{
    /// <summary>
    /// The key used to store the Guid in the HttpContext.Items dictionary
    /// </summary>
    public static readonly string GuidIdentifier = nameof(SessionMiddleware<TSessionData>) + ".Guid";

    private readonly RequestDelegate _next;

    private readonly JsonSerializerOptions _errorSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public SessionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, SessionService<TSessionData> sessionService, TokenService tokenService, LoginContext loginContext, RefGuidService dataReference)
    {
        var (path, method) = (context.Request.Path, context.Request.Method);

        Console.ForegroundColor = ConsoleColor.Cyan;

        if (!IsAuthorizedEndpoint(context))
        {
            Console.WriteLine($@"Request {method} {path} anonymous endpoint");

            await _next(context);
            return;
        }

        Console.WriteLine($@"Request {method} {path} authorized endpoint");

        // Retrieve the data reference from the JWT token
        dataReference.Value = context.User.GetDataReference();

        Console.ForegroundColor = ConsoleColor.White;

        try
        {
            var appUser = tokenService.CheckTokenChecksum(context, loginContext);

            // Attempt to capture data, this sets the InUse property to true
            // The capture fails if the SessionData object was desposed by a SignalR disconnect event
            if (!await sessionService.CaptureDataAsync())
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                throw new Exception("The session was closed, renew the token to initialize a new session");
            }

            var dataWrapper = sessionService.RetrieveDataWrapper();

            // Initialize the SessionData if it is not initialized yet (first request)
            if (!dataWrapper.IsInitialized)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($@"Request {method} {path} initializing session data");
                Console.ForegroundColor = ConsoleColor.White;

                // Only initialize the SessionData if the request is from a SignalR hub
                // This prevents the SessionData being initialized but not disposed e.g. when the SignalR connection is never established and therefor never closed/disposed
                if (context.Request.Path.StartsWithSegments("/hubs"))
                    dataWrapper.Init(appUser);
                else
                    throw new Exception(
                        "The Session is not fully initialized, start the session by connecting to a SignalR hub");
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($@"Request {method} {path} error: {ex.Message}");
            Console.ForegroundColor = ConsoleColor.White;

            // Only show error message if request is from localhost
            if (!context.Connection.RemoteIpAddress!.Equals(context.Connection.LocalIpAddress))
                return;

            // Don't show error message if request is from a SignalR hub
            if (context.Request.Path.StartsWithSegments("/hubs"))
                return;

            // Write the error message to the Http response as json
            await WriteError(context, ex);
        }
        finally
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($@"Request {method} {path} finally block reached");
            Console.ForegroundColor = ConsoleColor.White;

            if (dataReference.Value.HasValue)
                // Hub connection data reference was already released in OnConnectedAsync event
                if (!Regex.IsMatch(context.Request.Path, @"^\/hubs\/[^\/]+(?<!\/negotiate)$"))
                {
                    sessionService.ReleaseData();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($@"Session data released: {dataReference.Value}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }
    }
    private static bool IsAuthorizedEndpoint(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        return endpoint is not null &&
               // Check if the endpoint has AuthorizeAttribute or a subclass of it
               endpoint.Metadata.Any(metadataItem => metadataItem is AuthorizeAttribute) &&
               // Check if none AllowAnonymousAttribute or a subclass of it is present
               endpoint.Metadata.All(metadataItem => metadataItem is not AllowAnonymousAttribute);
    }

    private async Task WriteError(HttpContext context, Exception ex)
    {
        if (context.Response.HasStarted)
            return;

        if (context.Response.StatusCode < StatusCodes.Status400BadRequest)
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/json";

        var errorMessages = GetAllExceptions(ex)
            .Select(e => new
            {
                e.Source,
                e.Message,
                StackTrace = e.StackTrace?.Split(Environment.NewLine, StringSplitOptions.TrimEntries)
            })
            .ToList();

        var errorJson = JsonSerializer.Serialize(errorMessages, _errorSerializerOptions);

        await context.Response.WriteAsync(errorJson);
    }

    private static IEnumerable<Exception> GetAllExceptions(Exception? ex)
    {
        while (ex != null)
        {
            yield return ex;
            ex = ex.InnerException;
        }
    }
}
