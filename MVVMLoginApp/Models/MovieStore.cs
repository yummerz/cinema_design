using System.Collections.ObjectModel;

namespace MVVMLoginApp.Models
{
    public static class MovieStore
    {
        public static ObservableCollection<Movie> Movies { get; } = new ObservableCollection<Movie>
        {
            new Movie { Title = "Inception", Genre = "Sci-Fi", Duration = "148 min", Rating = "PG-13" },
            new Movie { Title = "The Dark Knight", Genre = "Action", Duration = "152 min", Rating = "PG-13" },
            new Movie { Title = "Interstellar", Genre = "Sci-Fi", Duration = "169 min", Rating = "PG" },
        };
    }
}