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

        private static readonly string connectionString = DatabaseConfig.ConnectionString;

        public ObservableCollection<Reservation> Reservations { get; } = new();
        public ObservableCollection<Showing> Showings { get; } = new();

        // ── Form title ──
        private string _formTitle = "New Reservation";
        public string FormTitle
        {
            get => _formTitle;
            set => SetProperty(ref _formTitle, value);
        }

        // ── Seats remaining display ──
        private string _seatsInfo = string.Empty;
        public string SeatsInfo
        {
            get => _seatsInfo;
            set => SetProperty(ref _seatsInfo, value);
        }

        // ── Selected reservation from the list ──
        private Reservation? _selectedReservation;
        public Reservation? SelectedReservation
        {
            get => _selectedReservation;
            set
            {
                SetProperty(ref _selectedReservation, value);
                if (_selectedReservation != null)
                {
                    ClientName = _selectedReservation.ClientName;
                    SelectedShowing = FindShowing(_selectedReservation.ShowingId);
                    FormTitle = "Update Reservation";
                }
                else
                {
                    FormTitle = "New Reservation";
                }
            }
        }

        // ── Selected showing from ComboBox ──
        private Showing? _selectedShowing;
        public Showing? SelectedShowing
        {
            get => _selectedShowing;
            set
            {
                SetProperty(ref _selectedShowing, value);

                // When a showing is selected update the seats info
                if (_selectedShowing != null)
                    _ = UpdateSeatsInfo(_selectedShowing);
                else
                    SeatsInfo = string.Empty;
            }
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

            _ = LoadAllDataFromDatabase();
        }

        // ── Helper to find Showing by ID ──
        private Showing? FindShowing(int showingId)
        {
            foreach (var showing in Showings)
                if (showing.ShowingId == showingId) return showing;
            return null;
        }

        // ── Check seats remaining for selected showing ──
        private async Task UpdateSeatsInfo(Showing showing)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Get room capacity and count existing reservations
                    string query = @"SELECT r.Capacity,
                                     (SELECT COUNT(*) FROM Reservations 
                                      WHERE ShowingId = @showingId) AS BookedSeats
                                     FROM Rooms r
                                     JOIN Showings s ON r.RoomId = s.RoomId
                                     WHERE s.ShowingId = @showingId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@showingId", showing.ShowingId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int capacity = (int)reader["Capacity"];
                                int bookedSeats = (int)reader["BookedSeats"];
                                int remaining = capacity - bookedSeats;

                                SeatsInfo = remaining > 0
                                    ? $"✅ {remaining} seat(s) remaining out of {capacity}"
                                    : $"❌ This showing is fully booked! ({capacity}/{capacity})";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to check seats: " + ex.Message);
            }
        }

        // ── Get next available seat number for a showing ──
        private async Task<int> GetNextSeatNumber(int showingId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT ISNULL(MAX(SeatNumber), 0) + 1 " +
                                   "FROM Reservations WHERE ShowingId = @showingId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@showingId", showingId);
                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to get seat number: " + ex.Message);
                return 0;
            }
        }

        // ── Check if showing is fully booked ──
        private async Task<bool> IsShowingFull(Showing showing)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT r.Capacity,
                                     (SELECT COUNT(*) FROM Reservations
                                      WHERE ShowingId = @showingId) AS BookedSeats
                                     FROM Rooms r
                                     JOIN Showings s ON r.RoomId = s.RoomId
                                     WHERE s.ShowingId = @showingId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@showingId", showing.ShowingId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int capacity = (int)reader["Capacity"];
                                int bookedSeats = (int)reader["BookedSeats"];
                                return bookedSeats >= capacity;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to check capacity: " + ex.Message);
            }
            return false;
        }

        // READ ALL
        private async Task LoadAllDataFromDatabase()
        {
            await LoadShowings();
            await LoadReservations();
        }

        private async Task LoadShowings()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT s.ShowingId, s.MovieId, s.RoomId,
                                     m.Title AS MovieTitle, r.RoomName,
                                     s.ShowingDate, s.ShowingTime
                                     FROM Showings s
                                     JOIN Movies m ON s.MovieId = m.MovieId
                                     JOIN Rooms  r ON s.RoomId  = r.RoomId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Showings.Add(new Showing
                                {
                                    ShowingId = (int)reader["ShowingId"],
                                    MovieId = (int)reader["MovieId"],
                                    RoomId = (int)reader["RoomId"],
                                    MovieTitle = reader["MovieTitle"].ToString()!,
                                    RoomName = reader["RoomName"].ToString()!,
                                    ShowingDate = (DateTime)reader["ShowingDate"],
                                    ShowingTime = (TimeSpan)reader["ShowingTime"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load showings: " + ex.Message);
            }
        }

        private async Task LoadReservations()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT r.ReservationId, r.ClientName, r.ShowingId,
                                     r.SeatNumber, m.Title AS MovieTitle, ro.RoomName,
                                     s.ShowingDate, s.ShowingTime
                                     FROM Reservations r
                                     JOIN Showings s  ON r.ShowingId = s.ShowingId
                                     JOIN Movies   m  ON s.MovieId   = m.MovieId
                                     JOIN Rooms    ro ON s.RoomId    = ro.RoomId";

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
                                    ShowingId = (int)reader["ShowingId"],
                                    SeatNumber = (int)reader["SeatNumber"],
                                    MovieTitle = reader["MovieTitle"].ToString()!,
                                    RoomName = reader["RoomName"].ToString()!,
                                    ShowingDate = (DateTime)reader["ShowingDate"],
                                    ShowingTime = (TimeSpan)reader["ShowingTime"]
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
            if (string.IsNullOrWhiteSpace(ClientName) || SelectedShowing == null) return;

            // Check if showing is full before proceeding
            if (await IsShowingFull(SelectedShowing))
            {
                MessageBox.Show(
                    $"Sorry! '{SelectedShowing.MovieTitle}' in {SelectedShowing.RoomName} " +
                    $"on {SelectedShowing.ShowingDate:MM/dd/yyyy} is fully booked.",
                    "Showing Full", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get next available seat number
            int seatNumber = await GetNextSeatNumber(SelectedShowing.ShowingId);

            var newReservation = new Reservation
            {
                ClientName = ClientName,
                ShowingId = SelectedShowing.ShowingId,
                SeatNumber = seatNumber,
                MovieTitle = SelectedShowing.MovieTitle,
                RoomName = SelectedShowing.RoomName,
                ShowingDate = SelectedShowing.ShowingDate,
                ShowingTime = SelectedShowing.ShowingTime
            };

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Reservations (ClientName, ShowingId, SeatNumber) " +
                                   "OUTPUT INSERTED.ReservationId " +
                                   "VALUES (@clientName, @showingId, @seatNumber)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@clientName", newReservation.ClientName);
                        command.Parameters.AddWithValue("@showingId", newReservation.ShowingId);
                        command.Parameters.AddWithValue("@seatNumber", newReservation.SeatNumber);

                        var result = await command.ExecuteScalarAsync();
                        newReservation.ReservationId = Convert.ToInt32(result);
                    }
                }

                Reservations.Add(newReservation);

                MessageBox.Show(
                    $"Reservation confirmed!\n" +
                    $"Client: {newReservation.ClientName}\n" +
                    $"Movie: {newReservation.MovieTitle}\n" +
                    $"Room: {newReservation.RoomName}\n" +
                    $"Seat: {newReservation.SeatNumber}",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if (SelectedReservation == null ||
                string.IsNullOrWhiteSpace(ClientName) ||
                SelectedShowing == null) return;

            var reservationToUpdate = SelectedReservation;

            // Only check capacity if changing to a different showing
            if (reservationToUpdate.ShowingId != SelectedShowing.ShowingId)
            {
                if (await IsShowingFull(SelectedShowing))
                {
                    MessageBox.Show(
                        $"Sorry! '{SelectedShowing.MovieTitle}' in {SelectedShowing.RoomName} " +
                        $"is fully booked.",
                        "Showing Full", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Assign new seat number for the new showing
                reservationToUpdate.SeatNumber = await GetNextSeatNumber(SelectedShowing.ShowingId);
            }

            reservationToUpdate.ClientName = ClientName;
            reservationToUpdate.ShowingId = SelectedShowing.ShowingId;
            reservationToUpdate.MovieTitle = SelectedShowing.MovieTitle;
            reservationToUpdate.RoomName = SelectedShowing.RoomName;
            reservationToUpdate.ShowingDate = SelectedShowing.ShowingDate;
            reservationToUpdate.ShowingTime = SelectedShowing.ShowingTime;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Reservations SET ClientName = @clientName, " +
                                   "ShowingId = @showingId, SeatNumber = @seatNumber " +
                                   "WHERE ReservationId = @id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@clientName", reservationToUpdate.ClientName);
                        command.Parameters.AddWithValue("@showingId", reservationToUpdate.ShowingId);
                        command.Parameters.AddWithValue("@seatNumber", reservationToUpdate.SeatNumber);
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

            // Refresh seats info after deletion
            if (SelectedShowing != null)
                await UpdateSeatsInfo(SelectedShowing);

            await ExecuteClear();
        }

        // CLEAR
        public async Task ExecuteClear()
        {
            ClientName = string.Empty;
            SelectedShowing = null;
            SelectedReservation = null;
            SeatsInfo = string.Empty;
            FormTitle = "New Reservation";
            await Task.CompletedTask;
        }
    }
}