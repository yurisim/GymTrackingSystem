using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GymTrackingSystem
{
 public class DurationToColor : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var input = (int) value;

                return input > 60 ? Brushes.Red : DependencyProperty.UnsetValue;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
}
