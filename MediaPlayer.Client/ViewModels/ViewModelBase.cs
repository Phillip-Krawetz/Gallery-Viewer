using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Input;
using MediaPlayer.Domain.Models;
using ReactiveUI;

namespace MediaPlayer.Client.ViewModels
{
  public class ViewModelBase : ReactiveObject
  {
    public virtual void Next(){}
    public virtual void Prev(){}
    public delegate void Back();
    public event Back BackEvent;
    public virtual void BackEventHandler()
    {
      BackEvent?.Invoke();
    }
  }
}
