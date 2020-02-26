using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MediaPlayer.Client.Converters
{
  public class PathToNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (value as string).Substring((value as string).LastIndexOf('\\') + 1);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}