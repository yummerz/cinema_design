using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //IsLoading = true;
            //ErrorMessage = string.Empty;

            //var user = _authService.Authenticate(Username, Password);

            //if (user is not null)
            //{
            //    _onLoginSuccess(user);
            //}
            //else
            //{
            //    ErrorMessage = "Invalid username or password. Please try again.";
            //}

            //IsLoading = false;

            string connectionString = @"Server=CCL2-11\MSSQLSERVER01; Database=Mawlers Cinema;
                        User Id=sa;Password=ccl2;
                        TrustServerCertificate=True;";
            bool isLoginValid = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) // ✅ create connection object
                {
                    string query = "SELECT * FROM Users WHERE Username = @username AND Password = @password";
                    using (SqlCommand command = new SqlCommand(query, connection)) // ✅ pass connection object
                    {
                        command.Parameters.AddWithValue("@username", Username.Trim());
                        command.Parameters.AddWithValue("@password", Password.Trim());
                        connection.Open(); // ✅ open the connection object
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
        } // ← closes ExecuteLogin()

    } // ← closes LoginViewModel class

    //public async Task ExecuteSaveCommand(object par)
    //{
    //    //append new item to your list through Add Method
    //    LostItemsList.Add(new LostItems
    //    {
    //        ItemName = newLostItems.ItemName,
    //        Description = newLostItems.Description,
    //        DateReported = newLostItems.DateReported,
    //        Location = newLostItems.Location,
    //        IsFound = newLostItems.IsFound
    //    });

    //    //behind the scene
    //    string connectionString = @"Server=CCL2-11\MSSQLSERVER01; Database=Mawlers Cinema;
    //                    User Id=sa;Password=ccl2;
    //                    TrustServerCertificate=True;";
    //    try
    //    {
    //        using (SqlConnection connection = new SqlConnection(connectionString)) // ✅ create connection object
    //        {
    //            string query = "INSERT INTO LostItemTable (ItemName, Description, Location, DateReported, IsFound) VALUES (@item,@desc,@loc,@datereported,@isFound)";
    //            using (SqlCommand command = new SqlCommand(query, connection)) // ✅ pass connection object
    //            {
    //                await connection.OpenAsync();
    //                command.Parameters.AddWithValue("@item", newLostItems.ItemName);
    //                command.Parameters.AddWithValue("@desc", newLostItems.Description);
    //                command.Parameters.AddWithValue("@loc", newLostItems.Location);
    //                command.Parameters.AddWithValue("@datereported", DateTime.Now);
    //                command.Parameters.AddWithValue("@isFound", newLostItems.IsFound);

    //                await command.ExecuteNonQueryAsync();

    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show("Database connection failed: " + ex.Message);
    //        return;
    //    }

    //    newLostItems.ItemName = String.Empty;
    //    newLostItems.Description = String.Empty;
    //    newLostItems.Location = string.Empty;
    //    newLostItems.DateReported = DateTime.Now;
    //    newLostItems.IsFound = false;

    //}

} // ← closes namespace



