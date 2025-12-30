namespace Slingcessories.Mobile.Maui.Services;

public class UserStateService
{
    private const string CurrentUserIdKey = "CurrentUserId";
    private string? _currentUserId;

    public UserStateService()
    {
        // Load persisted user ID on initialization
        _currentUserId = Preferences.Get(CurrentUserIdKey, null);
    }

    public string? CurrentUserId
    {
        get => _currentUserId;
        set
        {
            if (_currentUserId != value)
            {
                _currentUserId = value;
                
                // Persist to preferences
                if (string.IsNullOrEmpty(value))
                {
                    Preferences.Remove(CurrentUserIdKey);
                }
                else
                {
                    Preferences.Set(CurrentUserIdKey, value);
                }
                
                OnUserChanged?.Invoke();
            }
        }
    }

    public event Action? OnUserChanged;
}
