using CommonLib.Model;
using Microsoft.AspNetCore.Authorization;

namespace TestServiceGRPC.Utils;

public class RequireRoleAttribute : AuthorizeAttribute
{
    public RequireRoleAttribute(AppUserRole role) : base(role.ToString()) { }
}
