using Microsoft.Data.SqlClient;
using MVVMLoginApp.Commands;
using MVVMLoginApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MVVMLoginApp.ViewModels
{
    public class MakeaReservationViewModel : BaseViewModel
    {
        private readonly Action _onBackToMain;
        private readonly Action _onLogout;

        private static readonly string connectionString =
            @"Server=CCL2-11\MSSQLSERVER01; Database=Mawlers Cinema;
              User Id=sa; Password=ccl2;
              TrustServerCertificate=True;";

        public ObservableCollection<Movie> Movies => MovieStore.Movies;
        public ObservableCollection<Reservation> Reservations { get; } = new();

        private Movie? _selectedMovie;
        public Movie? SelectedMovie
        {
            get => _selectedMovie;
            set => SetProperty(ref _selectedMovie, value);
        }

        private string _clientName = string.Empty;
        public string ClientName
        {
            get => _clientName;
            set => SetProperty(ref _clientName, value);
        }

        public ICommand ConfirmCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackToMainMenuCommand { get; }
        public ICommand LogoutCommand { get; }

        public MakeaReservationViewModel(Action onBackToMain, Action onLogout)
        {
            _onBackToMain = onBackToMain;
            _onLogout = onLogout;

            ConfirmCommand = new AsyncRelayCommand(ExecuteConfirm);
            ClearCommand = new AsyncRelayCommand(ExecuteClear);
            BackToMainMenuCommand = new AsyncRelayCommand(() => Task.Run(_onBackToMain));
            LogoutCommand = new AsyncRelayCommand(() => Task.Run(_onLogout));
        }

        // CREATE
        public async Task ExecuteConfirm()
        {
            if (string.IsNullOrWhiteSpace(ClientName) || SelectedMovie == null) return;

            var newReservation = new Reservation
            {
                ClientName = ClientName,
                MovieTitle = SelectedMovie.Title
            };

            Reservations.Add(newReservation);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Reservations (ClientName, MovieTitle) " +
                                   "VALUES (@clientName, @movieTitle)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@clientName", newReservation.ClientName);
                        command.Parameters.AddWithValue("@movieTitle", newReservation.MovieTitle);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save reservation: " + ex.Message);
            }

            await ExecuteClear();
        }

        // CLEAR
        public async Task ExecuteClear()
        {
            ClientName = string.Empty;
            SelectedMovie = null;
            await Task.CompletedTask;
        }
    }
}