using Avalonia.Input;
using MediaPlayer.Domain.Variables;
using ReactiveUI;

namespace MediaPlayer.Client.ViewModels
{
  public class ViewModelBase : ReactiveObject
  {
    public KeyGesture NextKey
    {
      get => Options.NextKey;
    }

    public KeyGesture PrevKey
    {
      get => Options.PrevKey;
    }

    public KeyGesture BackKey
    {
      get => Options.BackKey;
    }

    public KeyGesture ConfirmKey
    {
      get => Options.ConfirmKey;
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
