using Avalonia.Controls;
using Avalonia.Input;

namespace MediaPlayer.Client.Abstracts
{
  public abstract class AbstractUserControlWithTags : UserControl
  {
    protected void TagHover(object sender, PointerEventArgs args)
    {
      var temp = (TextBlock)sender;
      temp.FontWeight = Avalonia.Media.FontWeight.Bold;
      args.Handled = true;
    }

    protected void TagUnhover(object sender, PointerEventArgs args)
    {
      var temp = (TextBlock)sender;
      temp.FontWeight = Avalonia.Media.FontWeight.Normal;
      args.Handled = true;
    }

    protected void TagHandler(object sender, PointerPressedEventArgs args)
    {
      switch(args.GetCurrentPoint(null).Properties.PointerUpdateKind)
      {
        case(PointerUpdateKind.LeftButtonPressed):
          TagLeftClick(sender, args);
          break;

        case(PointerUpdateKind.RightButtonPressed):
          TagRightClick(sender, args);
          break;
        case(PointerUpdateKind.MiddleButtonPressed):
          TagMiddleClick(sender, args);
          break;
      }
    }

    protected abstract void TagLeftClick(object sender, PointerPressedEventArgs args);

    protected abstract void TagRightClick(object sender, PointerPressedEventArgs args);

    protected abstract void TagMiddleClick(object sender, PointerPressedEventArgs args);
  }
}