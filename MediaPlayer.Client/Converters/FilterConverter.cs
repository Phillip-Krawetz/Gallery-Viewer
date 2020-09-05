using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MediaPlayer.Client.Converters
{
  public class FilterConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if((bool)value)
      {
        return new SolidColorBrush(){Color = new Color(255,255,150,150)};
      }
      return new SolidColorBrush(){Color = new Color(255,50,255,50)};
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}