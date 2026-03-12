using MVVMLoginApp.Models;
using System.Collections.ObjectModel;

namespace MVVMLoginApp.ViewModels
{
    public class LostItemViewModel : BaseViewModel
    {
        public ObservableCollection<LostItem> LostItems { get; set; }

        public LostItemViewModel()
        {
            LostItems = new ObservableCollection<LostItem>
            {
                new LostItem { Name = "Wallet", Description = "Black", LocationLost = "Library", DateReported = "5/10/2024" },
                new LostItem { Name = "Keys", Description = "Set of 3", LocationLost = "Cafeteria", DateReported = "5/12/2024" },
                new LostItem { Name = "Backpack", Description = "Blue with white stripes", LocationLost = "Gym", DateReported = "5/15/2024" },
                new LostItem { Name = "Phone", Description = "Phone 12", LocationLost = "Lecture Hall", DateReported = "5/18/2024" },
            };
        }
    }
}