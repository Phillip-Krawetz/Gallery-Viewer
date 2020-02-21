using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace MediaPlayer.Client.Views
{
  public class ImageView : UserControl
  {

    private Image image;
    public ImageView()
    {
      InitializeComponent();
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
      image = this.FindControl<Image>("img");
    }

    public void HoverHandler(object sender, PointerEventArgs args)
    {
      if(sender.GetType() == typeof(Button))
      {
        var o = (Button)sender;
        if(o.Opacity < 1)
        {
          o.Opacity = 0.8;
        }
      }
      args.Handled = true;
    }

    public void DehoverHandler(object sender, PointerEventArgs args)
    {
      if(sender.GetType() == typeof(Button))
      {
        var o = (Button)sender;
        if(o.Opacity != 0.2)
        {
          o.Opacity = 0.2;
        }
      }
      args.Handled = true;
    }
  }
}