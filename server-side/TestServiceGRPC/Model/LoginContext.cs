using CommonLib.Model;
using Microsoft.EntityFrameworkCore;

namespace TestServiceGRPC.Model;

public class LoginContext : DbContext
{
    public DbSet<AppUser> Users { get; set; } = null!;

    public DbSet<SessionToken> SessionTokens { get; set; } = null!;

    public LoginContext(DbContextOptions<LoginContext> options) : base(options) { }
}
