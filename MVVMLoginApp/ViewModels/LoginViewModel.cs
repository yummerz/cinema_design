using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMLoginApp.Commands;
using MVVMLoginApp.Models;
using System.Windows.Input;

namespace MVVMLoginApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly Action<User> _onLoginSuccess;

        // ── BINDABLE PROPERTIES ──────────────────────────────

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // ── COMMANDS ──────────────────────────────────────────
        public ICommand LoginCommand { get; }

        // ── CONSTRUCTOR ───────────────────────────────────────
        public LoginViewModel(AuthService authService, Action<User> onLoginSuccess)
        {
            _authService = authService;
            _onLoginSuccess = onLoginSuccess;

            LoginCommand = new RelayCommand(ExecuteLogin, CanLogin);
        }

        // ── COMMAND LOGIC ─────────────────────────────────────

        // CanLogin: The Login button is disabled if these are empty
        private bool CanLogin() =>
            !string.IsNullOrWhiteSpace(Username) &&
            !string.IsNullOrWhiteSpace(Password) &&
            !IsLoading;

        // ExecuteLogin: What happens when the button is clicked
        private void ExecuteLogin()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var user = _authService.Authenticate(Username, Password);

            if (user is not null)
            {
                _onLoginSuccess(user);
            }
            else
            {
                ErrorMessage = "Invalid username or password. Please try again.";
            }

            IsLoading = false;
        }
    }
}
