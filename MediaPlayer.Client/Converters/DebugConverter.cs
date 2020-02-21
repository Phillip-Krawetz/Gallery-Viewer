using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MediaPlayer.Client.Converters
{
    public class DebugDataBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
 
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}