using System;
using System.Reactive.Subjects;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using MediaPlayer.Client.Abstracts;
using MediaPlayer.Client.Converters;
using MediaPlayer.Client.ViewModels;
using MediaPlayer.Domain.Models;
using MediaPlayer.Domain.Variables;

namespace MediaPlayer.Client.Views
{
  public class ImageView : AbstractUserControlWithTags
  {
    private DockPanel imagePanel;
    private Image image;
    private Point start;
    private Point origin;
    private bool panning;
    public ImageView()
    {
      InitializeComponent();
      imagePanel = this.FindControl<DockPanel>("ImagePanel");
      imagePanel.PropertyChanged += ImageLoad;
      image = this.FindControl<Image>("img");
      panning = false;
      var tagList = this.FindControl<StackPanel>("TagPanel");
      tagList.Width = 0;
      if(Options.ShowTagSidebarOnImageView)
      {
        tagList.Width = 100;
      }
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    private void ImageLoad(object sender, AvaloniaPropertyChangedEventArgs args)
    {
      if(args.Property == DockPanel.BoundsProperty && (Rect?)args.OldValue == default(Rect))
      {
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(new ScaleTransform());
        transformGroup.Children.Add(new TranslateTransform());
        image.RenderTransform = transformGroup;
        if(Options.Preload)
        {
          image.Bind(Image.SourceProperty, new Binding("CurrentImage"));
        }
        else
        {
          var temp = new BitmapValueConverter();
          image.Bind(Image.SourceProperty, new Binding("ImagePath"){Converter = temp});
        }
        if(Options.Resize)
        {
          image.Height = imagePanel.Height;
          image.Width = imagePanel.Width;
        }
        else
        {
          image.Height = image.Source.Size.Height;
          image.Width = image.Source.Size.Width;
        }
      }
    }

    public void ImageScroll(object sender, PointerWheelEventArgs args)
    {
      var transform = (ScaleTransform)((TransformGroup)image.RenderTransform)
              .Children.First(x => x is ScaleTransform);
      var zoom = args.Delta.Y > 0 ? .2 : -.2;
      transform.ScaleX += zoom;
      transform.ScaleY += zoom;
    }

    public void BeginPan(object sender, PointerPressedEventArgs args)
    {
      var transform = (TranslateTransform)((TransformGroup)image.RenderTransform)
              .Children.First(x => x is TranslateTransform);
      start = args.GetPosition(this);
      origin = new Point(transform.X, transform.Y);
      panning = true;
    }

    public void Pan(object sender, PointerEventArgs args)
    {
      if(panning)
      {
        var transform = (TranslateTransform)((TransformGroup)image.RenderTransform)
              .Children.First(x => x is TranslateTransform);
        var vector = start - args.GetPosition(this);
        transform.X = origin.X - vector.X;
        transform.Y = origin.Y - vector.Y;
      }
    }

    public void EndPan(object sender, PointerReleasedEventArgs args)
    {
      panning = false;
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

    protected override void TagLeftClick(object sender, PointerPressedEventArgs args)
    {
      return;
    }

    protected override void TagRightClick(object sender, PointerPressedEventArgs args)
    {
      var popup = this.FindControl<Popup>("TagPopup");
      popup.PlacementTarget = sender as Control;
      popup.Open();
    }

    protected override void TagMiddleClick(object sender, PointerPressedEventArgs args)
    {
      return;
    }

    protected override void SetSelectedDirectory(TextBlock textBlock)
    {
      return;
    }

    private void TagRemove(object sender, PointerPressedEventArgs args)
    {
      var popup = (sender as TextBlock).Parent.Parent as Popup;
      if(popup != null)
      {
        var di = (this.DataContext as ImageViewModel).SelectedDirectory; 
        var tag = (popup.PlacementTarget as TextBlock).DataContext as Tag;
        var vm = this.DataContext as ImageViewModel;
        vm.RemoveTag(di, tag);
        popup.Close();
      }
    }
  }
}