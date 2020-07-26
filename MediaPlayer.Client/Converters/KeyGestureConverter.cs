using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Input;

namespace MediaPlayer.Client.Converters
{
  public class KeyGestureConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(value!=null)
      {
        return value.ToString();
      }
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var valString = value as string;
      var split = valString.Split(" + ");
      var myKey = new Key();
      var myModifier = new KeyModifiers();
      switch(split.Length)
      {
        case 1:
          Enum.TryParse(split[0], out myKey);
          break;
        case 2:
          Enum.TryParse(split[0], out myModifier);
          Enum.TryParse(split[1], out myKey);
          break;
        default:
          break;
      }
      return new KeyGesture(myKey, myModifier);
    }
  }
}