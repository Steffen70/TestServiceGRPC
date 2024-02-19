using CommonLib.Model;
using Microsoft.EntityFrameworkCore;
using TestServiceGRPC.Model;
using TestServiceGRPC.Services;
using TestServiceGRPC.Utils.Services;

namespace TestServiceGRPC.Utils.Extensions;

internal static class ServiceProviderExtensions
{
    public static IServiceProvider GetSessionScope(this IServiceProvider serviceProvider)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

        if (httpContextAccessor?.HttpContext == null)
            throw new Exception("The HTTP context is not available.");

        // Override the serviceProvider with the scoped serviceProvider from the HttpContext
        serviceProvider = httpContextAccessor.HttpContext.RequestServices;

        return serviceProvider;
    }

    public static TSessionData GetSessionData<TSessionData>(this IServiceProvider serviceProvider) where TSessionData : class, new()
    {
        var sessionService = serviceProvider.GetRequiredService<SessionService<TSessionData>>();
        var data = sessionService.RetrieveData();

        return data;
    }
    
    public static void SeedDataBase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var services = scope.ServiceProvider;

        var loginContext = services.GetRequiredService<LoginContext>();
        loginContext.Database.Migrate();

        if (loginContext.Users.Any()) return;

        var (password, _) = AuthService.CreateUser(loginContext, "steffen@seventy.mx", AppUserRole.Admin);

        Task.Run(async () =>
        {
            await Task.Delay(5000);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(@"Admin password: ");
            Console.ResetColor();
            Console.WriteLine($@"""{password}""");
        });
    }
}