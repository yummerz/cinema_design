using MVVMLoginApp.Commands;
using MVVMLoginApp.Models;
using System;
using System.Windows.Input;

namespace MVVMLoginApp.ViewModels
{
    public class CinemaMainViewModel : BaseViewModel
    {
        private readonly Action _onLogout;
        private readonly Action<string> _onNavigate;

        public ICommand LogoutCommand { get; }
        public ICommand ViewAllMoviesCommand { get; }
        public ICommand ViewAllShowingsCommand { get; }
        public ICommand ScheduleaShowingCommand { get; }
        public ICommand MakeaReservationCommand { get; }
        public ICommand ViewAvailableSeatsCommand { get; }
        public ICommand ViewAllReservationsCommand { get; }
        public ICommand ViewFirstandLastShowingCommand { get; }
        public ICommand ViewCinemaRoomsCommand { get; }
        public ICommand AddaMovieCommand { get; }
        public ICommand SaveandExitCommand { get; }

        public CinemaMainViewModel(Action onLogout, Action<string> onNavigate)
        {
            _onLogout = onLogout;
            _onNavigate = onNavigate;

            LogoutCommand = new RelayCommand(ExecuteLogout);
            ViewAllMoviesCommand = new RelayCommand(ExecuteViewAllMovies);
            ViewAllShowingsCommand = new RelayCommand(ExecuteViewAllShowings);
            ScheduleaShowingCommand = new RelayCommand(ExecuteScheduleaShowing);
            MakeaReservationCommand = new RelayCommand(ExecuteMakeaReservation);
            ViewAvailableSeatsCommand = new RelayCommand(ExecuteViewAvailableSeats);
            ViewAllReservationsCommand = new RelayCommand(ExecuteViewAllReservations);
            ViewFirstandLastShowingCommand = new RelayCommand(ExecuteViewFirstandLastShowing);
            ViewCinemaRoomsCommand = new RelayCommand(ExecuteViewCinemaRooms);
            AddaMovieCommand = new RelayCommand(ExecuteAddaMovie);
            SaveandExitCommand = new RelayCommand(ExecuteSaveandExit);
        }

        private void ExecuteLogout()
        {
            _onLogout();
        }

        private void ExecuteViewAllMovies()
        {
            _onNavigate("ViewAllMovies");
        }

        private void ExecuteViewAllShowings()
        {
            _onNavigate("ViewAllShowings");
        }

        private void ExecuteScheduleaShowing()
        {
            _onNavigate("ScheduleaShowing");
        }

        private void ExecuteMakeaReservation()
        {
            _onNavigate("MakeaReservation");
        }

        private void ExecuteViewAvailableSeats()
        {
            _onNavigate("ViewAvailableSeats");
        }

        private void ExecuteViewAllReservations()
        {
            _onNavigate("ViewAllReservations");
        }

        private void ExecuteViewFirstandLastShowing()
        {
            _onNavigate("ViewFirstandLastShowing");
        }

        private void ExecuteViewCinemaRooms()
        {
            _onNavigate("ViewCinemaRooms");
        }

        private void ExecuteAddaMovie()
        {
            _onNavigate("AddaMovie");
        }

        private void ExecuteSaveandExit()
        {
            _onNavigate("SaveandExit");
        }
    }
}