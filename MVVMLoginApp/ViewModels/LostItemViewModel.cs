using MVVMLoginApp.Models;
using System.Collections.ObjectModel;
using MVVMLoginApp.Commands;
using System.Windows.Input;

namespace MVVMLoginApp.ViewModels
{
    public class LostItemViewModel : BaseViewModel
    {
        public ObservableCollection<LostItem> LostItems { get; set; }

        public LostItemViewModel()
        {

            LostItems = new ObservableCollection<LostItem>
            {
                new LostItem { Name = "Wallet", Description = "Black", LocationLost = "Library", DateReported = "5/10/2024", Status = true },
                new LostItem { Name = "Keys", Description = "Set of 3", LocationLost = "Cafeteria", DateReported = "5/12/2024", Status = true },
                new LostItem { Name = "Backpack", Description = "Blue with white stripes", LocationLost = "Gym", DateReported = "5/15/2024", Status = false },
                new LostItem { Name = "Phone", Description = "Phone 12", LocationLost = "Lecture Hall", DateReported = "5/18/2024", Status = false },
            };
            SaveCommand = new RelayCommand(ExecuteSave);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            ClearCommand = new RelayCommand(ExecuteClear);
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _locationLost = string.Empty;
        public string LocationLost
        {
            get => _locationLost;
            set => SetProperty(ref _locationLost, value);
        }

        private DateTime? _dateReported = DateTime.MinValue;
        public DateTime? DateReported
        {
            get => _dateReported;
            set => SetProperty(ref _dateReported, value);
        }

        private bool _status;
        public bool Status
        {
            get => _status;

            set => SetProperty(ref _status, value);
        }

        private LostItem? _selectedItem;
        public LostItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                if (_selectedItem != null)
                {
                    Name = _selectedItem.Name;
                    Description = _selectedItem.Description;
                    LocationLost = _selectedItem.LocationLost;
                    DateReported = DateTime.Parse(_selectedItem.DateReported);
                    Status = _selectedItem.Status;
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        private void ExecuteSave()
        {
            LostItems.Add(new LostItem
            {
                Name = Name,
                Description = Description,
                LocationLost = LocationLost,
                DateReported = DateReported?.ToString("M/d/yyyy") ?? string.Empty,
                Status = Status
            });
            ExecuteClear();
        }

        private void ExecuteDelete()
        {
            if (SelectedItem != null)
            {
                LostItems.Remove(SelectedItem);
                ExecuteClear();
            }
        }

        private void ExecuteClear()
        {
            Name = string.Empty;
            Description = string.Empty;
            LocationLost = string.Empty;
            DateReported = null;
            Status = false;
            SelectedItem = null;
        }


    }
}