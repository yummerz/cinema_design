using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

// ⚠ Replace "MVVMLoginApp" below with YOUR project name if different
namespace MVVMLoginApp.Converters
{
    // Returns Visible if the string has text, Collapsed if it is empty.
    // Used to show/hide the error message label automatically.
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
