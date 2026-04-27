using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVMLoginApp.Models
{
    public class Showing : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _showingId;
        public int ShowingId
        {
            get => _showingId;
            set { _showingId = value; OnPropertyChanged(); }
        }

        private int _movieId;
        public int MovieId
        {
            get => _movieId;
            set { _movieId = value; OnPropertyChanged(); }
        }

        private int _roomId;
        public int RoomId
        {
            get => _roomId;
            set { _roomId = value; OnPropertyChanged(); }
        }

        // Display names for the UI
        private string _movieTitle = string.Empty;
        public string MovieTitle
        {
            get => _movieTitle;
            set
            {
                _movieTitle = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText)); // ← add this
            }
        }

        private string _roomName = string.Empty;
        public string RoomName
        {
            get => _roomName;
            set
            {
                _roomName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText)); // ← add this
            }
        }

        private DateTime _showingDate = DateTime.Today;
        public DateTime ShowingDate
        {
            get => _showingDate;
            set { _showingDate = value; OnPropertyChanged(); }
        }

        private TimeSpan _showingTime;
        public TimeSpan ShowingTime
        {
            get => _showingTime;
            set
            {
                _showingTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowingTimeDisplay)); // ← add this line
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        // Formatted time for display in the list
        public string ShowingTimeDisplay => ShowingTime.ToString(@"hh\:mm");
        // Combined display string for the ComboBox
        public string DisplayText => $"{MovieTitle} | {RoomName} | {ShowingDate:MM/dd/yyyy} {ShowingTimeDisplay}";
    }
}