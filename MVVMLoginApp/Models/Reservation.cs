using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVMLoginApp.Models
{
    public class Reservation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _reservationId;
        public int ReservationId
        {
            get => _reservationId;
            set { _reservationId = value; OnPropertyChanged(); }
        }

        private string _clientName = string.Empty;
        public string ClientName
        {
            get => _clientName;
            set { _clientName = value; OnPropertyChanged(); }
        }

        private string _movieTitle = string.Empty;
        public string MovieTitle
        {
            get => _movieTitle;
            set { _movieTitle = value; OnPropertyChanged(); }
        }
    }
}