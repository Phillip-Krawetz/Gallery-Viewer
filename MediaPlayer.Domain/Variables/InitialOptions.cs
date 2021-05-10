using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Input;

namespace MediaPlayer.Domain.Variables
{
  public class InitialOptions
  {
    public bool Preload {get; set; }

    public bool UseBackup { get; set; }

    public bool ShowTagSidebarOnImageView { get; set; }

    public bool Resize { get; set; }

    public KeyGesture NextKey {get; set; }

    public KeyGesture PrevKey {get; set; }

    public KeyGesture BackKey {get; set; }

    public KeyGesture ConfirmKey {get; set; }

    public static List<PropertyInfo> Properties
    {
      get => typeof(InitialOptions).GetProperties().ToList();
    }

    public void SetDefaults()
    {
      Preload = false;
      UseBackup = true;
      ShowTagSidebarOnImageView = false;
      Resize = true;
      NextKey = new KeyGesture(Key.Right, KeyModifiers.None);
      PrevKey = new KeyGesture(Key.Left, KeyModifiers.None);
      BackKey = new KeyGesture(Key.Left, KeyModifiers.Control);
      ConfirmKey = new KeyGesture(Key.Enter, KeyModifiers.None);
    }

    public InitialOptions(){}

    public void CopyCurrent()
    {
      var properties = this.GetType().GetProperties();
      foreach(var item in typeof(Options).GetProperties(BindingFlags.Public | BindingFlags.Static))
      {
        properties.Single(x => x.Name == item.Name).SetValue(this, item.GetValue(null));
      }
    }

    public void MapToOptions()
    {
      var properties = typeof(Options).GetProperties(BindingFlags.Public | BindingFlags.Static);
      foreach(var item in this.GetType().GetProperties())
      {
        if(properties.FirstOrDefault(x => x.Name == item.Name) != null){
          properties.Single(x => x.Name == item.Name).SetValue(null, item.GetValue(this));
        }
      }
    }
  }
}