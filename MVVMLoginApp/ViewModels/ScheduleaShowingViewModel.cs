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
    public class ScheduleaShowingViewModel : BaseViewModel
    {
        private readonly Action _onBackToMain;
        private readonly Action _onLogout;

        private static readonly string connectionString = DatabaseConfig.ConnectionString;

        public ObservableCollection<Showing> Showings { get; } = new();
        public ObservableCollection<Movie> Movies { get; } = new();
        public ObservableCollection<Room> Rooms { get; } = new();

        // ── Form title ──
        private string _formTitle = "Schedule New Showing";
        public string FormTitle
        {
            get => _formTitle;
            set => SetProperty(ref _formTitle, value);
        }

        // ── Selected showing from the list ──
        private Showing? _selectedShowing;
        public Showing? SelectedShowing
        {
            get => _selectedShowing;
            set
            {
                SetProperty(ref _selectedShowing, value);
                if (_selectedShowing != null)
                {
                    SelectedMovie = FindMovie(_selectedShowing.MovieId);
                    SelectedRoom = FindRoom(_selectedShowing.RoomId);
                    ShowingDate = _selectedShowing.ShowingDate;
                    ShowingTime = _selectedShowing.ShowingTime.ToString(@"hh\:mm");
                    FormTitle = "Update Showing";
                }
                else
                {
                    FormTitle = "Schedule New Showing";
                }
            }
        }

        // ── ComboBox selections ──
        private Movie? _selectedMovie;
        public Movie? SelectedMovie
        {
            get => _selectedMovie;
            set => SetProperty(ref _selectedMovie, value);
        }

        private Room? _selectedRoom;
        public Room? SelectedRoom
        {
            get => _selectedRoom;
            set => SetProperty(ref _selectedRoom, value);
        }

        // ── Date and Time fields ──
        private DateTime _showingDate = DateTime.Today;
        public DateTime ShowingDate
        {
            get => _showingDate;
            set => SetProperty(ref _showingDate, value);
        }

        // Time as string so TextBox can bind to it e.g. "14:30"
        private string _showingTime = string.Empty;
        public string ShowingTime
        {
            get => _showingTime;
            set => SetProperty(ref _showingTime, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackToMainMenuCommand { get; }
        public ICommand LogoutCommand { get; }

        public ScheduleaShowingViewModel(Action onBackToMain, Action onLogout)
        {
            _onBackToMain = onBackToMain;
            _onLogout = onLogout;

            SaveCommand = new AsyncRelayCommand(ExecuteSave);
            UpdateCommand = new AsyncRelayCommand(ExecuteUpdate);
            DeleteCommand = new AsyncRelayCommand(ExecuteDelete);
            ClearCommand = new AsyncRelayCommand(ExecuteClear);
            BackToMainMenuCommand = new AsyncRelayCommand(() => Task.Run(_onBackToMain));
            LogoutCommand = new AsyncRelayCommand(() => Task.Run(_onLogout));

            _ = LoadAllDataFromDatabase();
        }

        // ── Helpers to find Movie/Room by ID for ComboBox ──
        private Movie? FindMovie(int movieId)
        {
            foreach (var movie in Movies)
                if (movie.MovieId == movieId) return movie;
            return null;
        }

        private Room? FindRoom(int roomId)
        {
            foreach (var room in Rooms)
                if (room.RoomId == roomId) return room;
            return null;
        }

        // READ ALL — loads Movies, Rooms and Showings from database
        private async Task LoadAllDataFromDatabase()
        {
            await LoadMovies();
            await LoadRooms();
            await LoadShowings();
        }

        private async Task LoadMovies()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT MovieId, Title, Genre, Duration, Rating FROM Movies";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Movies.Add(new Movie
                                {
                                    MovieId = (int)reader["MovieId"],
                                    Title = reader["Title"].ToString()!,
                                    Genre = reader["Genre"].ToString()!,
                                    Duration = reader["Duration"].ToString()!,
                                    Rating = reader["Rating"].ToString()!
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load movies: " + ex.Message);
            }
        }

        private async Task LoadRooms()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT RoomId, RoomName, Capacity FROM Rooms";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Rooms.Add(new Room
                                {
                                    RoomId = (int)reader["RoomId"],
                                    RoomName = reader["RoomName"].ToString()!,
                                    Capacity = (int)reader["Capacity"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load rooms: " + ex.Message);
            }
        }

        private async Task LoadShowings()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // JOIN with Movies and Rooms to get display names
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

        // CREATE
        public async Task ExecuteSave()
        {
            if (SelectedMovie == null || SelectedRoom == null ||
                string.IsNullOrWhiteSpace(ShowingTime)) return;

            if (!TimeSpan.TryParse(ShowingTime, out TimeSpan timeValue))
            {
                MessageBox.Show("Please enter a valid time in HH:MM format (e.g. 14:30).");
                return;
            }

            var newShowing = new Showing
            {
                MovieId = SelectedMovie.MovieId,
                RoomId = SelectedRoom.RoomId,
                MovieTitle = SelectedMovie.Title,
                RoomName = SelectedRoom.RoomName,
                ShowingDate = ShowingDate,
                ShowingTime = timeValue
            };

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Showings (MovieId, RoomId, ShowingDate, ShowingTime) " +
                                   "OUTPUT INSERTED.ShowingId " +
                                   "VALUES (@movieId, @roomId, @date, @time)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@movieId", newShowing.MovieId);
                        command.Parameters.AddWithValue("@roomId", newShowing.RoomId);
                        command.Parameters.AddWithValue("@date", newShowing.ShowingDate);
                        command.Parameters.AddWithValue("@time", newShowing.ShowingTime);

                        var result = await command.ExecuteScalarAsync();
                        newShowing.ShowingId = Convert.ToInt32(result);
                    }
                }

                Showings.Add(newShowing);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save showing: " + ex.Message);
            }

            await ExecuteClear();
        }

        // UPDATE
        public async Task ExecuteUpdate()
        {
            if (SelectedShowing == null || SelectedMovie == null ||
                SelectedRoom == null || string.IsNullOrWhiteSpace(ShowingTime)) return;

            if (!TimeSpan.TryParse(ShowingTime, out TimeSpan timeValue))
            {
                MessageBox.Show("Please enter a valid time in HH:MM format (e.g. 14:30).");
                return;
            }

            var showingToUpdate = SelectedShowing;
            showingToUpdate.MovieId = SelectedMovie.MovieId;
            showingToUpdate.RoomId = SelectedRoom.RoomId;
            showingToUpdate.MovieTitle = SelectedMovie.Title;
            showingToUpdate.RoomName = SelectedRoom.RoomName;
            showingToUpdate.ShowingDate = ShowingDate;
            showingToUpdate.ShowingTime = timeValue;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Showings SET MovieId = @movieId, RoomId = @roomId, " +
                                   "ShowingDate = @date, ShowingTime = @time " +
                                   "WHERE ShowingId = @id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@movieId", showingToUpdate.MovieId);
                        command.Parameters.AddWithValue("@roomId", showingToUpdate.RoomId);
                        command.Parameters.AddWithValue("@date", showingToUpdate.ShowingDate);
                        command.Parameters.AddWithValue("@time", showingToUpdate.ShowingTime);
                        command.Parameters.AddWithValue("@id", showingToUpdate.ShowingId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update showing: " + ex.Message);
            }

            await ExecuteClear();
        }

        // DELETE
        // DELETE
        public async Task ExecuteDelete()
        {
            if (SelectedShowing == null) return;

            // Check if showing has any reservations first
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string checkQuery = "SELECT COUNT(*) FROM Reservations WHERE ShowingId = @id";
                    using (SqlCommand command = new SqlCommand(checkQuery, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@id", SelectedShowing.ShowingId);
                        int count = (int)await command.ExecuteScalarAsync();

                        if (count > 0)
                        {
                            MessageBox.Show(
                                $"Cannot delete this showing of '{SelectedShowing.MovieTitle}' " +
                                $"because it has {count} reservation(s) linked to it. " +
                                "Please delete those reservations first.",
                                "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking showing usage: " + ex.Message);
                return;
            }

            // Safe to delete — no reservations attached
            var showingToDelete = SelectedShowing;
            Showings.Remove(showingToDelete);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Showings WHERE ShowingId = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@id", showingToDelete.ShowingId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete showing: " + ex.Message);
            }

            await ExecuteClear();
        }

        // CLEAR
        public async Task ExecuteClear()
        {
            SelectedMovie = null;
            SelectedRoom = null;
            ShowingDate = DateTime.Today;
            ShowingTime = string.Empty;
            SelectedShowing = null;
            FormTitle = "Schedule New Showing";
            await Task.CompletedTask;
        }
    }
}