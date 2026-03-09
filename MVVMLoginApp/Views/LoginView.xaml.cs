using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MVVMLoginApp.ViewModels;
using System.Windows.Controls;

namespace MVVMLoginApp.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();

            // When the password changes, push it to the ViewModel
            PasswordInput.PasswordChanged += (s, e) =>
            {
                if (DataContext is LoginViewModel vm)
                    vm.Password = PasswordInput.Password;
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
