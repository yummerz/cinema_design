using MVVMLoginApp.Commands;
using MVVMLoginApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MVVMLoginApp.ViewModels
{
    public class ViewAllMoviesViewModel : BaseViewModel
    {
        private readonly Action _onBackToMain;
        private readonly Action _onLogout;

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

            SaveCommand = new RelayCommand(ExecuteSave);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            ClearCommand = new RelayCommand(ExecuteClear);
            BackToMainMenuCommand = new RelayCommand(_onBackToMain);
            LogoutCommand = new RelayCommand(_onLogout);
        }

        private void ExecuteSave()
        {
            if (string.IsNullOrWhiteSpace(Title)) return;
            MovieStore.Movies.Add(new Movie
            {
                Title = Title,
                Genre = Genre,
                Duration = Duration,
                Rating = Rating
            });
            ExecuteClear();
        }

        private void ExecuteDelete()
        {
            if (SelectedMovie != null)
            {
                MovieStore.Movies.Remove(SelectedMovie);
                ExecuteClear();
            }
        }

        private void ExecuteClear()
        {
            Title = string.Empty;
            Genre = string.Empty;
            Duration = string.Empty;
            Rating = string.Empty;
            SelectedMovie = null;
        }
    }
}