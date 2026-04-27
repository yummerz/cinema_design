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
        public ICommand MakeaReservationCommand { get; }
        public ICommand ViewCinemaRoomsCommand { get; }
        public ICommand ScheduleaShowingCommand { get; }

        public CinemaMainViewModel(Action onLogout, Action<string> onNavigate)
        {
            _onLogout = onLogout;
            _onNavigate = onNavigate;

            LogoutCommand = new RelayCommand(ExecuteLogout);
            ViewAllMoviesCommand = new RelayCommand(ExecuteViewAllMovies);
            MakeaReservationCommand = new RelayCommand(ExecuteMakeaReservation);
            ViewCinemaRoomsCommand = new RelayCommand(ExecuteViewCinemaRooms);
            ScheduleaShowingCommand = new RelayCommand(ExecuteScheduleaShowing);
        }

        private void ExecuteLogout()
        {
            _onLogout();
        }

        private void ExecuteViewAllMovies()
        {
            _onNavigate("ViewAllMovies");
        }

        private void ExecuteMakeaReservation()
        {
            _onNavigate("MakeaReservation");
        }
        private void ExecuteViewCinemaRooms()
        {
            _onNavigate("ViewCinemaRooms");
        }
        private void ExecuteScheduleaShowing()
        {
            _onNavigate("ScheduleaShowing");
        }

    }
}