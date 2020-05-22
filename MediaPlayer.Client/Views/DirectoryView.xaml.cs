using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MediaPlayer.Domain.Models;
using MediaPlayer.Client.ViewModels;
using System.IO;
using System;
using Avalonia.Media;

namespace MediaPlayer.Client.Views
{
  public class DirectoryView : UserControl
  {
    private DataGrid grid;
    private static object lastObject;
    private DockPanel control;
    public DirectoryView()
    {
      InitializeComponent();
      grid = this.FindControl<DataGrid>("DirectoryGrid");
      grid.PropertyChanged += SetOffset;
      control = this.FindControl<DockPanel>("MainPanel");
      control.PropertyChanged += SetGridHeight;
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    private void SetGridHeight(object sender, AvaloniaPropertyChangedEventArgs args)
    {
      if(args.Property == DockPanel.BoundsProperty)
      {
        grid.Height = control.Height;
        grid.Width = control.Width - 100;
      }
    }

    private void SetOffset(object sender, AvaloniaPropertyChangedEventArgs args)
    {
      if(args.Property == DataGrid.TransformedBoundsProperty)
      {
        if(args.OldValue == null)
        {
          grid.ScrollIntoView(lastObject, null);
        }
      }
    }

    private void TagHandler(object sender, PointerPressedEventArgs args)
    {
      var dc = this.DataContext as DirectoryViewModel;
      var tag = (sender as StyledElement).DataContext;
      switch(args.GetCurrentPoint(null).Properties.PointerUpdateKind)
      {
        case(PointerUpdateKind.LeftButtonPressed):
          dc.AddFilter(tag as Tag);
          break;

        case(PointerUpdateKind.RightButtonPressed):
          var popup = this.FindControl<Popup>("Popup");
          popup.PlacementTarget = sender as Control;
          popup.Open();
          break;
      }
    }

    private void TagRemove(object sender, PointerPressedEventArgs args)
    {
      var popup = (sender as TextBlock).Parent.Parent as Popup;
      if(popup != null)
      {
        var di = (popup.PlacementTarget as TextBlock).Parent.Parent.DataContext as DirectoryItem;
        var tag = (popup.PlacementTarget as TextBlock).DataContext as Tag;
        var vm = this.DataContext as DirectoryViewModel;
        vm.RemoveTag(di, tag);
        popup.Close();
      }
    }

    private void TagHover(object sender, PointerEventArgs args)
    {
      var temp = (TextBlock)sender;
      temp.FontStyle = Avalonia.Media.FontStyle.Italic;
      args.Handled = true;
    }

    private void TagUnhover(object sender, PointerEventArgs args)
    {
      var temp = (TextBlock)sender;
      temp.FontStyle = Avalonia.Media.FontStyle.Normal;
      args.Handled = true;
    }

    private void AddTag(object sender, PointerPressedEventArgs args)
    {
      var textBlock = (TextBlock)sender;
      var vm = (DirectoryViewModel)this.DataContext;
      var textBox = (textBlock.Parent as WrapPanel).Children.First(x => x.GetType() == typeof(TextBox)) as TextBox;
      vm.AddTag((DirectoryItem)textBlock.DataContext, textBox.Text);
      textBox.Text = "";
      args.Handled = true;
    }

    private void RemoveFilter(object sender, PointerPressedEventArgs args)
    {
      var dc = this.DataContext as DirectoryViewModel;
      dc.RemoveFilter((sender as StyledElement).DataContext as Tag);
    }

    private void SelectionHandler(object sender, DataGridCellPointerPressedEventArgs args)
    {
      IDataContextProvider obj = args.Cell.Content as IDataContextProvider;
      if(obj==null)
      {
        return;
      }
      var dc = obj.DataContext;
      if(args.Column.Header.ToString() != "Tags")
      {
        if(this.Parent.Name == "MainContent")
        {
          if(dc.GetType() == typeof(DirectoryItem))
          {
            lastObject = dc;
            var temp = (DirectoryItem)dc;
            if(File.Exists(temp.StartPath))
            {
              var vm = new ImageViewModel(temp.StartPath);
              var t = (MainWindowViewModel)this.Parent.DataContext;
              t.Content = vm;
            }
          }
        }
        return;
      }
    }
  }
}