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
    public class ViewCinemaRoomsViewModel : BaseViewModel
    {
        private readonly Action _onBackToMain;
        private readonly Action _onLogout;

        private static readonly string connectionString = DatabaseConfig.ConnectionString;

        public ObservableCollection<Room> Rooms { get; } = new();

        // ── Form title changes between New and Update mode ──
        private string _formTitle = "Add New Room";
        public string FormTitle
        {
            get => _formTitle;
            set => SetProperty(ref _formTitle, value);
        }

        // ── Selected room from the list ──
        private Room? _selectedRoom;
        public Room? SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                SetProperty(ref _selectedRoom, value);
                if (_selectedRoom != null)
                {
                    RoomName = _selectedRoom.RoomName;
                    Capacity = _selectedRoom.Capacity.ToString();
                    FormTitle = "Update Room";
                }
                else
                {
                    FormTitle = "Add New Room";
                }
            }
        }

        private string _roomName = string.Empty;
        public string RoomName
        {
            get => _roomName;
            set => SetProperty(ref _roomName, value);
        }

        // Capacity is a string in the form so the TextBox can bind to it
        private string _capacity = string.Empty;
        public string Capacity
        {
            get => _capacity;
            set => SetProperty(ref _capacity, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackToMainMenuCommand { get; }
        public ICommand LogoutCommand { get; }

        public ViewCinemaRoomsViewModel(Action onBackToMain, Action onLogout)
        {
            _onBackToMain = onBackToMain;
            _onLogout = onLogout;

            SaveCommand = new AsyncRelayCommand(ExecuteSave);
            UpdateCommand = new AsyncRelayCommand(ExecuteUpdate);
            DeleteCommand = new AsyncRelayCommand(ExecuteDelete);
            ClearCommand = new AsyncRelayCommand(ExecuteClear);
            BackToMainMenuCommand = new AsyncRelayCommand(() => Task.Run(_onBackToMain));
            LogoutCommand = new AsyncRelayCommand(() => Task.Run(_onLogout));

            _ = LoadRoomsFromDatabase();
        }

        // READ
        private async Task LoadRoomsFromDatabase()
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

        // CREATE
        public async Task ExecuteSave()
        {
            if (string.IsNullOrWhiteSpace(RoomName)) return;
            if (!int.TryParse(Capacity, out int capacityValue))
            {
                MessageBox.Show("Please enter a valid number for Capacity.");
                return;
            }

            var newRoom = new Room
            {
                RoomName = RoomName,
                Capacity = capacityValue
            };

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Rooms (RoomName, Capacity) " +
                                   "OUTPUT INSERTED.RoomId " +
                                   "VALUES (@roomName, @capacity)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@roomName", newRoom.RoomName);
                        command.Parameters.AddWithValue("@capacity", newRoom.Capacity);

                        var result = await command.ExecuteScalarAsync();
                        newRoom.RoomId = Convert.ToInt32(result);
                    }
                }

                Rooms.Add(newRoom);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save room: " + ex.Message);
            }

            await ExecuteClear();
        }

        // UPDATE
        public async Task ExecuteUpdate()
        {
            if (SelectedRoom == null || string.IsNullOrWhiteSpace(RoomName)) return;
            if (!int.TryParse(Capacity, out int capacityValue))
            {
                MessageBox.Show("Please enter a valid number for Capacity.");
                return;
            }

            var roomToUpdate = SelectedRoom;
            roomToUpdate.RoomName = RoomName;
            roomToUpdate.Capacity = capacityValue;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Rooms SET RoomName = @roomName, " +
                                   "Capacity = @capacity " +
                                   "WHERE RoomId = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@roomName", roomToUpdate.RoomName);
                        command.Parameters.AddWithValue("@capacity", roomToUpdate.Capacity);
                        command.Parameters.AddWithValue("@id", roomToUpdate.RoomId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update room: " + ex.Message);
            }

            await ExecuteClear();
        }

        // DELETE in ViewCinemaRoomsViewModel
        public async Task ExecuteDelete()
        {
            if (SelectedRoom == null) return;

            // Check if room is used in any showings first
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string checkQuery = "SELECT COUNT(*) FROM Showings WHERE RoomId = @id";
                    using (SqlCommand command = new SqlCommand(checkQuery, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@id", SelectedRoom.RoomId);
                        int count = (int)await command.ExecuteScalarAsync();

                        if (count > 0)
                        {
                            MessageBox.Show(
                                $"Cannot delete '{SelectedRoom.RoomName}' because it has {count} showing(s) scheduled. " +
                                "Please delete those showings first.",
                                "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking room usage: " + ex.Message);
                return;
            }

            // Safe to delete — no showings attached
            var roomToDelete = SelectedRoom;
            Rooms.Remove(roomToDelete);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Rooms WHERE RoomId = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@id", roomToDelete.RoomId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete room: " + ex.Message);
            }

            await ExecuteClear();
        }


        // CLEAR
        public async Task ExecuteClear()
        {
            RoomName = string.Empty;
            Capacity = string.Empty;
            SelectedRoom = null;
            FormTitle = "Add New Room";
            await Task.CompletedTask;
        }
    }
}