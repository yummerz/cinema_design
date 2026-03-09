using MVVMLoginApp.Commands;
using System.Windows.Input;

namespace MVVMLoginApp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        // 1. Add bindable properties
        private string _someValue = string.Empty;
        public string SomeValue
        {
            get => _someValue;
            set => SetProperty(ref _someValue, value);
        }

        // 2. Add Commands for user actions
        public ICommand SaveCommand { get; }

        // 3. Constructor — receive dependencies via parameters
        public SettingsViewModel(Action onBack)
        {
            SaveCommand = new RelayCommand(() =>
            {
                // Your save logic here
                onBack(); // Navigate back when done
            });
        }
    }
}