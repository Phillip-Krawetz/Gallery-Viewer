using Avalonia.Input;
using ReactiveUI;

namespace MediaPlayer.Client.ViewModels
{
  public class ViewModelBase : ReactiveObject
  {
    public KeyGesture NextKey
    {
      get => new KeyGesture(Key.Right, KeyModifiers.None);
    }

    public KeyGesture PrevKey
    {
      get => new KeyGesture(Key.Left, KeyModifiers.None);
    }

    public KeyGesture BackKey
    {
      get => new KeyGesture(Key.Left, KeyModifiers.Control);
    }

    public KeyGesture ConfirmKey
    {
      get => new KeyGesture(Key.Enter, KeyModifiers.None);
    }

    public virtual void Next(){}
    public virtual void Prev(){}

    public delegate void Back();
    public event Back BackEvent;
    public virtual void BackEventHandler()
    {
      BackEvent?.Invoke();
    }
    
    public delegate void Confirm();
    public event Confirm ConfirmEvent;
    public Confirm confirmPress;
    public virtual void ConfirmEventHandler()
    {
      ConfirmEvent?.Invoke();
    }
  }
}
