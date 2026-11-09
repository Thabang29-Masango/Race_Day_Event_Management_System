using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RaceDayManagementSystemMVC.Helpers;


namespace RaceDayManagementSystemMVC.Filters;

/// <summary>
/// Session-based guard. [RequireRole] = any logged-in user,
/// [RequireRole("Organiser")] = that role only.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    public RequireRoleAttribute(params string[] roles) => _roles = roles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;

        if (!session.IsLoggedIn())
        {
            var request = context.HttpContext.Request;
            var returnUrl = request.Method == "GET"
                ? request.Path + request.QueryString
                : null;
            context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
            return;
        }

        if (_roles.Length > 0 && !_roles.Contains(session.GetRole()))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
        }
    }
}