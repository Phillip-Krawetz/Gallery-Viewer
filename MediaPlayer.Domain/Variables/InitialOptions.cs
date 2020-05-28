using System.Linq;
using System.Reflection;
using Avalonia.Input;

namespace MediaPlayer.Domain.Variables
{
  public class InitialOptions
  {
    public bool Preload {get; set; }

    public KeyGesture NextKey {get; set; }

    public KeyGesture PrevKey {get; set; }

    public KeyGesture BackKey {get; set; }

    public KeyGesture ConfirmKey {get; set; }

    public void SetDefaults()
    {
      Preload = false;
      NextKey = new KeyGesture(Key.Right, KeyModifiers.None);
      PrevKey = new KeyGesture(Key.Left, KeyModifiers.None);
      BackKey = new KeyGesture(Key.Left, KeyModifiers.Control);
      ConfirmKey = new KeyGesture(Key.Enter, KeyModifiers.None);
    }

    public InitialOptions(){}

    public void MapToOptions()
    {
      var properties = typeof(Options).GetProperties(BindingFlags.Public | BindingFlags.Static);
      foreach(var item in this.GetType().GetProperties())
      {
        properties.Single(x => x.Name == item.Name).SetValue(null, item.GetValue(this));
      }
    }
  }
}