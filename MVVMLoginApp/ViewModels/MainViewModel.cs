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
            // Pages will be added here as we build them
        }
    }
}