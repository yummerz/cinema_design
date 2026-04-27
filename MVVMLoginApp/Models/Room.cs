using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVMLoginApp.Models
{
    public class Room : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _roomId;
        public int RoomId
        {
            get => _roomId;
            set { _roomId = value; OnPropertyChanged(); }
        }

        private string _roomName = string.Empty;
        public string RoomName
        {
            get => _roomName;
            set { _roomName = value; OnPropertyChanged(); }
        }

        private int _capacity;
        public int Capacity
        {
            get => _capacity;
            set { _capacity = value; OnPropertyChanged(); }
        }
    }
}