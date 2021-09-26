using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using MediaPlayer.Client.ViewModels;

namespace MediaPlayer.Client.Abstracts
{
  public abstract class AbstractUserControlWithText : UserControl
  {
    protected void InitializeTextBindings()
    {
      DataContextChanged += SetBindings;
    }

    private void SetBindings(object sender, System.EventArgs args)
    {
      var dc = this.DataContext as ViewModelBase;
      if(dc == null)
      {
        return;
      }
      dc.NextEvent += TextRight;
      dc.PrevEvent += TextLeft;
    }
    protected void TextRight(RoutedEventArgs args)
    {
      var focus = FocusManager.Instance.Current;
      if(focus is TextBox)
      {
        var f = focus as TextBox;
        f.CaretIndex++;
        args.Handled = true;
      }
    }
    protected void TextLeft(RoutedEventArgs args)
    {
      var focus = FocusManager.Instance.Current;
      if(focus is TextBox)
      {
        var f = focus as TextBox;
        f.CaretIndex--;
        args.Handled = true;
      }
    }
  }
}