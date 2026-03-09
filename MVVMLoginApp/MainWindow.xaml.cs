using MVVMLoginApp.ViewModels;
using System.Windows;

namespace MVVMLoginApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Set the DataContext — this is what binds everything together
            DataContext = new MainViewModel();
        }
    }
}