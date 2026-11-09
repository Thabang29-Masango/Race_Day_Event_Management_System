using RaceDayManagementSystemMVC.Models;

namespace RaceDayManagementSystemMVC.Helpers;

public static class SessionKeys
{
    public const string Token = "Token";
    public const string UserId = "UserId";
    public const string Email = "Email";
    public const string Role = "Role";
    public const string FullName = "FullName";
}

public static class RaceDaySessionExtensions
{
    public static void SignIn(this ISession session, LoginResponse login)
    {
        session.Clear();
        session.SetString(SessionKeys.Token, login.Token);
        session.SetInt32(SessionKeys.UserId, login.UserId);
        session.SetString(SessionKeys.Email, login.Email);
        session.SetString(SessionKeys.Role, login.Role);
        session.SetString(SessionKeys.FullName, login.Email);
    }

    public static bool IsLoggedIn(this ISession session) =>
        !string.IsNullOrEmpty(session.GetString(SessionKeys.Token));

    public static string GetRole(this ISession session) =>
        session.GetString(SessionKeys.Role) ?? string.Empty;

    public static int GetUserId(this ISession session) =>
        session.GetInt32(SessionKeys.UserId) ?? 0;

    public static string GetFullName(this ISession session) =>
        session.GetString(SessionKeys.FullName) ?? string.Empty;
}

public static class Formatting
{
    public static string Ordinal(int n)
    {
        var lastTwo = n % 100;
        if (lastTwo is >= 11 and <= 13) return $"{n}th";
        return (n % 10) switch { 1 => $"{n}st", 2 => $"{n}nd", 3 => $"{n}rd", _ => $"{n}th" };
    }

    public static string Time(TimeSpan? t) =>
        t == null ? "-" : t.Value.ToString(@"hh\:mm\:ss");
}