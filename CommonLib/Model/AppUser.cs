using System.Security.Cryptography;
using System.Text;

namespace CommonLib.Model;

public class AppUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;

    public string? TokenChecksum { get; set; }

    public AppUserRole UserRole { get; set; } = AppUserRole.Member;

    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;


    public static AppUser Create(string email, string password, AppUserRole userRole = AppUserRole.Member)
    {
        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            Email = email.ToLower(),
            UserRole = userRole,
            PasswordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password))),
            PasswordSalt = Convert.ToBase64String(hmac.Key),
        };

        return user;
    }
}