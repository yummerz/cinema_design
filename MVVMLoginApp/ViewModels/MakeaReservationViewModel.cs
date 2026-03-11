using MVVMLoginApp.Commands;
using System;
using System.Windows.Input;

namespace MVVMLoginApp.ViewModels
{
    public class MakeaReservationViewModel : BaseViewModel
    {
        private readonly Action _onBackToMainMenu;
        private readonly Action _onLogout;

        public ICommand BackToMainMenuCommand { get; }
        public ICommand LogoutCommand { get; }

        public MakeaReservationViewModel(Action onBackToMainMenu, Action onLogout)
        {
            _onBackToMainMenu = onBackToMainMenu;
            _onLogout = onLogout;

            BackToMainMenuCommand = new RelayCommand(ExecuteBackToMainMenu);
            LogoutCommand = new RelayCommand(ExecuteLogout);
        }

        private void ExecuteBackToMainMenu()
        {
            _onBackToMainMenu();
        }

        private void ExecuteLogout()
        {
            _onLogout();
        }
    }
}