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

        // ── Form title changes depending on mode ──
        private string _formTitle = "New Reservation";
        public string FormTitle
        {
            get => _formTitle;
            set => SetProperty(ref _formTitle, value);
        }

        // ── Selected reservation from the list ──
        private Reservation? _selectedReservation;
        public Reservation? SelectedReservation
        {
            get => _selectedReservation;
            set
            {
                SetProperty(ref _selectedReservation, value);

                // When a reservation is selected, populate the form fields
                if (_selectedReservation != null)
                {
                    ClientName = _selectedReservation.ClientName;
                    SelectedMovie = Movies.Count > 0
                        ? FindMovie(_selectedReservation.MovieTitle)
                        : null;
                    FormTitle = "Update Reservation"; // ← switch title to Update mode
                }
                else
                {
                    FormTitle = "New Reservation"; // ← switch back to New mode
                }
            }
        }

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
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackToMainMenuCommand { get; }
        public ICommand LogoutCommand { get; }

        public MakeaReservationViewModel(Action onBackToMain, Action onLogout)
        {
            _onBackToMain = onBackToMain;
            _onLogout = onLogout;

            ConfirmCommand = new AsyncRelayCommand(ExecuteConfirm);
            UpdateCommand = new AsyncRelayCommand(ExecuteUpdate);
            DeleteCommand = new AsyncRelayCommand(ExecuteDelete);
            ClearCommand = new AsyncRelayCommand(ExecuteClear);
            BackToMainMenuCommand = new AsyncRelayCommand(() => Task.Run(_onBackToMain));
            LogoutCommand = new AsyncRelayCommand(() => Task.Run(_onLogout));

            _ = LoadReservationsFromDatabase();
        }

        // Helper — finds a Movie object by title for the ComboBox
        private Movie? FindMovie(string title)
        {
            foreach (var movie in Movies)
            {
                if (movie.Title == title) return movie;
            }
            return null;
        }

        // READ
        private async Task LoadReservationsFromDatabase()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT ReservationId, ClientName, MovieTitle FROM Reservations";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Reservations.Add(new Reservation
                                {
                                    ReservationId = (int)reader["ReservationId"],
                                    ClientName = reader["ClientName"].ToString()!,
                                    MovieTitle = reader["MovieTitle"].ToString()!
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load reservations: " + ex.Message);
            }
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

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Reservations (ClientName, MovieTitle) " +
                                   "OUTPUT INSERTED.ReservationId " +
                                   "VALUES (@clientName, @movieTitle)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@clientName", newReservation.ClientName);
                        command.Parameters.AddWithValue("@movieTitle", newReservation.MovieTitle);

                        var result = await command.ExecuteScalarAsync();
                        newReservation.ReservationId = Convert.ToInt32(result);
                    }
                }

                Reservations.Add(newReservation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save reservation: " + ex.Message);
            }

            await ExecuteClear();
        }

        // UPDATE
        public async Task ExecuteUpdate()
        {
            // Must have a reservation selected and fields filled
            if (SelectedReservation == null ||
                string.IsNullOrWhiteSpace(ClientName) ||
                SelectedMovie == null) return;

            var reservationToUpdate = SelectedReservation;

            // Update the object in the UI list
            reservationToUpdate.ClientName = ClientName;
            reservationToUpdate.MovieTitle = SelectedMovie.Title;

            // Update in database
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Reservations SET ClientName = @clientName, " +
                                   "MovieTitle = @movieTitle " +
                                   "WHERE ReservationId = @id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@clientName", reservationToUpdate.ClientName);
                        command.Parameters.AddWithValue("@movieTitle", reservationToUpdate.MovieTitle);
                        command.Parameters.AddWithValue("@id", reservationToUpdate.ReservationId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update reservation: " + ex.Message);
            }

            await ExecuteClear();
        }

        // DELETE
        public async Task ExecuteDelete()
        {
            if (SelectedReservation == null) return;

            var reservationToDelete = SelectedReservation;
            Reservations.Remove(reservationToDelete);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Reservations WHERE ReservationId = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@id", reservationToDelete.ReservationId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete reservation: " + ex.Message);
            }

            await ExecuteClear();
        }

        // CLEAR
        public async Task ExecuteClear()
        {
            ClientName = string.Empty;
            SelectedMovie = null;
            SelectedReservation = null;
            FormTitle = "New Reservation";
            await Task.CompletedTask;
        }
    }
}