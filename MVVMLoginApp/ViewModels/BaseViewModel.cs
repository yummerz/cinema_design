using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVMLoginApp.ViewModels
{
    // This is our base class. Every ViewModel inherits from this.
    // It implements INotifyPropertyChanged so WPF knows when
    // to update the UI.
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Call this method inside your property setters
        // to notify WPF that the value changed.
        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }

        // A handy helper to set a value and auto-notify.
        // Returns true if the value actually changed.
        protected bool SetProperty<T>(ref T field, T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}