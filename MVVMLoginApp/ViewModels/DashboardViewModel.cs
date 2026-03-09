using MVVMLoginApp.Commands;
using MVVMLoginApp.Models;
using System.Windows.Input;

namespace MVVMLoginApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public string WelcomeMessage { get; }
        public ICommand LogoutCommand { get; }

        public DashboardViewModel(User loggedInUser, Action onLogout)
        {
            WelcomeMessage = $"Welcome, {loggedInUser.DisplayName}!";
            LogoutCommand = new RelayCommand(onLogout);
        }
    }
}