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
    public class ViewAllMoviesViewModel : BaseViewModel
    {
        private readonly Action _onBackToMain;
        private readonly Action _onLogout;

        private static readonly string connectionString =
            @"Server=CCL2-11\MSSQLSERVER01; Database=Mawlers Cinema;
              User Id=sa; Password=ccl2;
              TrustServerCertificate=True;";

        public ObservableCollection<Movie> Movies => MovieStore.Movies;

        private Movie? _selectedMovie;
        public Movie? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                SetProperty(ref _selectedMovie, value);
                if (_selectedMovie != null)
                {
                    Title = _selectedMovie.Title;
                    Genre = _selectedMovie.Genre;
                    Duration = _selectedMovie.Duration;
                    Rating = _selectedMovie.Rating;
                }
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _genre = string.Empty;
        public string Genre
        {
            get => _genre;
            set => SetProperty(ref _genre, value);
        }

        private string _duration = string.Empty;
        public string Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        private string _rating = string.Empty;
        public string Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackToMainMenuCommand { get; }
        public ICommand LogoutCommand { get; }

        public ViewAllMoviesViewModel(Action onBackToMain, Action onLogout)
        {
            _onBackToMain = onBackToMain;
            _onLogout = onLogout;

            SaveCommand = new AsyncRelayCommand(ExecuteSave);
            DeleteCommand = new AsyncRelayCommand(ExecuteDelete);
            ClearCommand = new AsyncRelayCommand(ExecuteClear);
            BackToMainMenuCommand = new AsyncRelayCommand(() => Task.Run(_onBackToMain));
            LogoutCommand = new AsyncRelayCommand(() => Task.Run(_onLogout));
        }

        // CREATE or UPDATE
        public async Task ExecuteSave()
        {
            if (string.IsNullOrWhiteSpace(Title)) return;

            // ── UPDATE path — a movie was selected from the list ──
            if (SelectedMovie != null)
            {
                var movieToUpdate = SelectedMovie;

                movieToUpdate.Title = Title;
                movieToUpdate.Genre = Genre;
                movieToUpdate.Duration = Duration;
                movieToUpdate.Rating = Rating;

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE Movies SET Title = @title, Genre = @genre, " +
                                       "Duration = @duration, Rating = @rating " +
                                       "WHERE MovieId = @id";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            await connection.OpenAsync();
                            command.Parameters.AddWithValue("@title", movieToUpdate.Title);
                            command.Parameters.AddWithValue("@genre", movieToUpdate.Genre);
                            command.Parameters.AddWithValue("@duration", movieToUpdate.Duration);
                            command.Parameters.AddWithValue("@rating", movieToUpdate.Rating);
                            command.Parameters.AddWithValue("@id", movieToUpdate.MovieId);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to update movie: " + ex.Message);
                }
            }
            // ── CREATE path — nothing selected, brand new movie ──
            else
            {
                var newMovie = new Movie
                {
                    Title = Title,
                    Genre = Genre,
                    Duration = Duration,
                    Rating = Rating
                };

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // OUTPUT INSERTED.MovieId returns the new ID that SQL generated
                        string query = "INSERT INTO Movies (Title, Genre, Duration, Rating) " +
                                       "OUTPUT INSERTED.MovieId " +
                                       "VALUES (@title, @genre, @duration, @rating)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            await connection.OpenAsync();
                            command.Parameters.AddWithValue("@title", newMovie.Title);
                            command.Parameters.AddWithValue("@genre", newMovie.Genre);
                            command.Parameters.AddWithValue("@duration", newMovie.Duration);
                            command.Parameters.AddWithValue("@rating", newMovie.Rating);

                            // Read back the real MovieId that the database generated
                            var result = await command.ExecuteScalarAsync();
                            newMovie.MovieId = Convert.ToInt32(result);
                        }
                    }

                    // Add to UI list AFTER we have the real MovieId
                    MovieStore.Movies.Add(newMovie);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save movie: " + ex.Message);
                }
            }

            await ExecuteClear();
        }

        // DELETE
        public async Task ExecuteDelete()
        {
            if (SelectedMovie == null) return;

            var movieToDelete = SelectedMovie;
            MovieStore.Movies.Remove(movieToDelete);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Movies WHERE MovieId = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@id", movieToDelete.MovieId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete movie: " + ex.Message);
            }

            await ExecuteClear();
        }

        // CLEAR
        public async Task ExecuteClear()
        {
            Title = string.Empty;
            Genre = string.Empty;
            Duration = string.Empty;
            Rating = string.Empty;
            SelectedMovie = null;
            await Task.CompletedTask;
        }
    }
}