using CommonLib.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TestServiceGRPC.Hubs;
using TestServiceGRPC.Middleware;
using TestServiceGRPC.Model;
using TestServiceGRPC.Utils;
using TestServiceGRPC.Utils.Extensions;
using TestServiceGRPC.Utils.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LoginContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("LoginContext"));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<InMemorySessionPool<SessionData>>();

builder.Services.AddScoped<RefGuidService>();

builder.Services.AddScoped<SessionService<SessionData>>();

builder.Services.AddScoped(ServiceProviderExtensions.GetSessionData<SessionData>);

builder.Services.AddTransient<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.ConfigureJwtBearer(TokenService.TokenKey));

builder.Services.AddSingleton<IAuthorizationPolicyProvider, RoleBasedAuthorizationPolicyProvider>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

builder.Services.AddGrpc();

builder.Services.AddControllers();

builder.Services.AddSignalR();

var app = builder.Build();

// Current seeded user is steffen@seventy.mx with password "bentclub72"
app.Services.SeedDataBase();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<SessionMiddleware<SessionData>>();

app.MapHub<SessionPersistenceHub<SessionData>>("/hubs/session-hub");

app.MapGrpcServices();

app.MapControllers();

app.Run();
