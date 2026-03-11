using MVVMLoginApp.Models;
namespace MVVMLoginApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private User? _currentUser;
        private BaseViewModel? _currentView;

        public BaseViewModel? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public MainViewModel()
        {
            _authService = new AuthService();
            NavigateToLogin();
        }

        private void NavigateToLogin()
        {
            CurrentView = new LoginViewModel(_authService, OnLoginSuccess);
        }

        private void OnLoginSuccess(User user)
        {
            _currentUser = user;
            NavigateToCinemaMain();
        }

        private void NavigateToCinemaMain()
        {
            CurrentView = new CinemaMainViewModel(NavigateToLogin, NavigateTo);
        }

        private void NavigateTo(string page)
        {
            CurrentView = page switch
            {
                "ViewAllMovies" => new ViewAllMoviesViewModel(NavigateToCinemaMain, NavigateToLogin),
                "ViewAllShowings" => new ViewAllShowingsViewModel(NavigateToCinemaMain, NavigateToLogin),
                "ScheduleaShowing" => new ScheduleaShowingViewModel(NavigateToCinemaMain, NavigateToLogin),
                "MakeaReservation" => new MakeaReservationViewModel(NavigateToCinemaMain, NavigateToLogin),
                "ViewAvailableSeats" => new ViewAvailableSeatsViewModel(NavigateToCinemaMain, NavigateToLogin),
                "ViewAllReservations" => new ViewAllReservationsViewModel(NavigateToCinemaMain, NavigateToLogin),
                "ViewFirstandLastShowing" => new ViewFirstandLastShowingViewModel(NavigateToCinemaMain, NavigateToLogin),
                "ViewCinemaRooms" => new ViewCinemaRoomsViewModel(NavigateToCinemaMain, NavigateToLogin),
                "AddaMovie" => new AddaMovieViewModel(NavigateToCinemaMain, NavigateToLogin),
                "SaveandExit" => new SaveandExitViewModel(NavigateToCinemaMain, NavigateToLogin),
                _ => CurrentView
            };
        }
    }
}