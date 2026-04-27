namespace OldandNewClone.Application.Services;

/// <summary>
/// Scoped service (one per Blazor circuit / MAUI session) that tracks the currently
/// logged-in user and their JWT access token.
/// </summary>
public class UserStateService
{
    public string? UserId { get; private set; }
    public string? Username { get; private set; }
    public string? Email { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public bool IsAdmin { get; private set; }
    public string? AccessToken { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);

    public string DisplayName =>
        !string.IsNullOrEmpty(FirstName) ? $"{FirstName} {LastName}".Trim()
        : Username ?? Email ?? "User";

    public event Action? OnChange;

    public void SetUser(string userId, string username, string email,
        string firstName, string lastName, bool isAdmin, string accessToken)
    {
        UserId = NormalizeUserId(userId);
        Username = username;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsAdmin = isAdmin;
        AccessToken = accessToken;
        OnChange?.Invoke();
    }

    public void Logout()
    {
        UserId = null;
        Username = null;
        Email = null;
        FirstName = null;
        LastName = null;
        IsAdmin = false;
        AccessToken = null;
        OnChange?.Invoke();
    }

    private static string NormalizeUserId(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return string.Empty;
        }

        const string objectIdPrefix = "ObjectId(\"";
        const string objectIdSuffix = "\")";

        if (userId.StartsWith(objectIdPrefix, StringComparison.Ordinal) && userId.EndsWith(objectIdSuffix, StringComparison.Ordinal))
        {
            return userId.Substring(objectIdPrefix.Length, userId.Length - objectIdPrefix.Length - objectIdSuffix.Length);
        }

        return userId;
    }
}
