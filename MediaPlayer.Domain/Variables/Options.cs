using Avalonia.Input;

namespace MediaPlayer.Domain.Variables
{
  public class Options
  {
    public static bool Preload {get; set; }

    public static KeyGesture NextKey {get; set; }

    public static KeyGesture PrevKey {get; set; }

    public static KeyGesture BackKey {get; set; }

    public static KeyGesture ConfirmKey {get; set; }
  }
}