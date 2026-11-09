using System.Net.Http.Headers;
using RaceDayManagementSystemMVC.Helpers;

namespace RaceDayManagementSystemMVC.Services;

/// <summary>Adds the logged-in user's JWT (from session) to every API call.</summary>
public class AuthTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _accessor;

    public AuthTokenHandler(IHttpContextAccessor accessor) => _accessor = accessor;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _accessor.HttpContext?.Session.GetString(SessionKeys.Token);
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, cancellationToken);
    }
}