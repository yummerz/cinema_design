using System;
using MVVMLoginApp.Commands;
using MVVMLoginApp.Models;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace MVVMLoginApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly Action<User> _onLoginSuccess;

        // ── CONNECTION STRING ─────────────────────────────────
        private static readonly string connectionString = DatabaseConfig.ConnectionString;

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
        private bool CanLogin() =>
            !string.IsNullOrWhiteSpace(Username) &&
            !string.IsNullOrWhiteSpace(Password) &&
            !IsLoading;

        private void ExecuteLogin()
        {
            bool isLoginValid = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Users WHERE Username = @username AND Password = @password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", Username.Trim());
                        command.Parameters.AddWithValue("@password", Password.Trim());
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                isLoginValid = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection failed: " + ex.Message);
                return;
            }

            if (isLoginValid)
            {
                var user = _authService.Authenticate(Username, Password);
                _onLoginSuccess(user);
            }
            else
            {
                MessageBox.Show("Invalid Username or Password.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}