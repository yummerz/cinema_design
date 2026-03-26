using System.Windows;

namespace MVVMLoginApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show(ex.Exception.Message + "\n\n" + ex.Exception.StackTrace, "Crash Details");
                ex.Handled = true;
            };
        }
    }
}