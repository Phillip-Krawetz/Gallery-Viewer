using System;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using MediaPlayer.Client.Converters;
using MediaPlayer.Client.ViewModels;
using MediaPlayer.Domain.Variables;

namespace MediaPlayer.Client.Views
{
  public class ImageView : UserControl
  {
    private DockPanel imagePanel;
    public ImageView()
    {
      InitializeComponent();
      imagePanel = this.FindControl<DockPanel>("MainPanel");
      imagePanel.PropertyChanged += ImageLoad;
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    private void ImageLoad(object sender, AvaloniaPropertyChangedEventArgs args)
    {
      if(args.Property == DockPanel.BoundsProperty && (Rect?)args.OldValue == default(Rect))
      {
        var img = this.FindControl<Image>("img");
        if(Options.Preload)
        {
          img.Bind(Image.SourceProperty, new Binding("CurrentImage"));
        }
        else
        {
          var temp = new BitmapValueConverter();
          img.Bind(Image.SourceProperty, new Binding("ImagePath"){Converter = temp});
        }
      }
    }

    public void HoverHandler(object sender, PointerEventArgs args)
    {
      var obj = sender as Visual;
      if(obj != null)
      {
        if(obj.Opacity < 1)
        {
          obj.Opacity = 1;
        }
      }
      args.Handled = true;
    }

    public void DehoverHandler(object sender, PointerEventArgs args)
    {
      var obj = sender as Visual;
      if(obj != null)
      {
        if(obj.Opacity != 0)
        {
          obj.Opacity = 0;
        }
      }
      args.Handled = true;
    }

    public void OpenPageSelect(object sender, PointerPressedEventArgs args)
    {
      var s = (Border)sender;
      if(s != null)
      {
        var popup = this.FindControl<Popup>("Popup");
        popup.Open();
      }
    }

    public void PageSelection(object sender, SelectionChangedEventArgs args)
    {
      var popup = this.FindControl<Popup>("Popup");
      popup.Close();
      var dc = this.DataContext as ImageViewModel;
      if(dc != null)
      {
        dc.JumpToPage(Convert.ToInt32(args.AddedItems[0]));
      }
    }
  }
}