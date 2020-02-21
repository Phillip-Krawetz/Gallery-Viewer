using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace MediaPlayer.Client.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
      public virtual void Next(){}
      public virtual void Prev(){}
    }
}
