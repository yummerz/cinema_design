using MVVMLoginApp.Commands;
using MVVMLoginApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MVVMLoginApp.ViewModels
{
    public class MakeaReservationViewModel : BaseViewModel
    {
        private readonly Action _onBackToMain;
        private readonly Action _onLogout;

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

            ConfirmCommand = new RelayCommand(ExecuteConfirm);
            ClearCommand = new RelayCommand(ExecuteClear);
            BackToMainMenuCommand = new RelayCommand(_onBackToMain);
            LogoutCommand = new RelayCommand(_onLogout);
        }

        private void ExecuteConfirm()
        {
            if (string.IsNullOrWhiteSpace(ClientName) || SelectedMovie == null) return;
            Reservations.Add(new Reservation
            {
                ClientName = ClientName,
                MovieTitle = SelectedMovie.Title
            });
            ExecuteClear();
        }

        private void ExecuteClear()
        {
            ClientName = string.Empty;
            SelectedMovie = null;
        }
    }
}