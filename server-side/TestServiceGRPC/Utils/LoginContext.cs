using CommonLib.Model;
using Microsoft.EntityFrameworkCore;

namespace TestServiceGRPC.Utils;

public class LoginContext : DbContext
{
    public DbSet<AppUser> Users { get; set; } = null!;

    public LoginContext(DbContextOptions<LoginContext> options) : base(options) { }
}
