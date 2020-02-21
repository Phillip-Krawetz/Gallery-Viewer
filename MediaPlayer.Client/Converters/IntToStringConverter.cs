using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MediaPlayer.Client.Converters
{
  public class IntToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((int)value + 1).ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int ret = 0;
      return int.TryParse((string)value, out ret) ? ret : 0;
    }
  }
}