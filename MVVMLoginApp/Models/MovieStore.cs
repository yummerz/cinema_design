using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace MVVMLoginApp.Models
{
    public static class MovieStore
    {
        private static readonly string connectionString =
            @"Server=CCL2-11\MSSQLSERVER01; Database=Mawlers Cinema;
              User Id=sa; Password=ccl2;
              TrustServerCertificate=True;";

        public static ObservableCollection<Movie> Movies { get; } = LoadMoviesFromDatabase();

        private static ObservableCollection<Movie> LoadMoviesFromDatabase()
        {
            var movies = new ObservableCollection<Movie>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT MovieId, Title, Genre, Duration, Rating FROM Movies";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                movies.Add(new Movie
                                {
                                    MovieId = (int)reader["MovieId"],
                                    Title = reader["Title"].ToString(),
                                    Genre = reader["Genre"].ToString(),
                                    Duration = reader["Duration"].ToString(),
                                    Rating = reader["Rating"].ToString()
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
            return movies;
        }
    }
}