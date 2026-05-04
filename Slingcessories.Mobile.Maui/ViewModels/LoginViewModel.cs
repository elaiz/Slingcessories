using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Slingcessories.Mobile.Maui.Services;

namespace Slingcessories.Mobile.Maui.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private readonly UserStateService _userStateService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _token = string.Empty;

    [ObservableProperty]
    private string _newPassword = string.Empty;

    [ObservableProperty]
    private bool _showReset;

    [ObservableProperty]
    private string? _message;

    [ObservableProperty]
    private bool _isBusy;

    public LoginViewModel(AuthService authService, UserStateService userStateService)
    {
        _authService = authService;
        _userStateService = userStateService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            Message = "Email and password are required.";
            return;
        }

        try
        {
            IsBusy = true;
            Message = null;

            var auth = await _authService.LoginAsync(Email.Trim(), Password);
            if (auth is null)
            {
                Message = "Invalid email or password.";
                return;
            }

            _userStateService.CurrentUserId = auth.UserId;
            await Shell.Current.GoToAsync("//AccessoriesPage");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            Message = "Enter your email first.";
            return;
        }

        try
        {
            IsBusy = true;
            Message = null;

            var resetToken = await _authService.ForgotPasswordAsync(Email.Trim());
            ShowReset = true;
            Token = resetToken ?? string.Empty;
            Message = resetToken is null
                ? "If the account exists, reset instructions were generated."
                : "Development reset token generated. Paste token and set a new password.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ResetPasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token) || string.IsNullOrWhiteSpace(NewPassword))
        {
            Message = "Email, reset token, and new password are required.";
            return;
        }

        try
        {
            IsBusy = true;
            Message = null;

            var ok = await _authService.ResetPasswordAsync(Email.Trim(), Token, NewPassword);
            Message = ok ? "Password reset successful." : "Reset failed.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
