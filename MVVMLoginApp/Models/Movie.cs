using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVMLoginApp.Models
{
    public class Movie : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _movieId;
        public int MovieId
        {
            get => _movieId;
            set { _movieId = value; OnPropertyChanged(); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _genre = string.Empty;
        public string Genre
        {
            get => _genre;
            set { _genre = value; OnPropertyChanged(); }
        }

        private string _duration = string.Empty;
        public string Duration
        {
            get => _duration;
            set { _duration = value; OnPropertyChanged(); }
        }

        private string _rating = string.Empty;
        public string Rating
        {
            get => _rating;
            set { _rating = value; OnPropertyChanged(); }
        }
    }
}