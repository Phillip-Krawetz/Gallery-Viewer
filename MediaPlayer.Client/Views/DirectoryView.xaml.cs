using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MediaPlayer.Domain.Models;
using MediaPlayer.Client.ViewModels;
using System.IO;
using Avalonia.VisualTree;
using MediaPlayer.Client.Models;

namespace MediaPlayer.Client.Views
{
  public class DirectoryView : UserControl
  {
    private DataGrid grid;
    private static object lastObject;
    private DockPanel control;
    private AutoCompleteBox manualFilter;
    public DirectoryView()
    {
      InitializeComponent();
      grid = this.FindControl<DataGrid>("DirectoryGrid");
      grid.PropertyChanged += SetOffset;
      control = this.FindControl<DockPanel>("MainPanel");
      control.PropertyChanged += SetGridHeight;
      manualFilter = this.FindControl<AutoCompleteBox>("ManualFilter");
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    private void ConfirmPressHappened(AutoCompleteBox sender)
    {
      AddTag(sender.DataContext as DirectoryItem, sender.Text);
      sender.Text = "";
      sender.ClearValue(AutoCompleteBox.TextProperty);
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
        case(PointerUpdateKind.MiddleButtonPressed):
          dc.AddFilter(tag as Tag, true);
          break;
      }
    }

    private void TagEdit(object sender, PointerPressedEventArgs args)
    {
      var popup = (sender as TextBlock).Parent.Parent as Popup;
      if(popup != null)
      {
        var tag = (popup.PlacementTarget as TextBlock).DataContext as Tag;
        var tagWindow = TagWindow.GetTagWindow;
        var openPoint = (this.Parent.Parent.Parent as Window).Position;
        openPoint = openPoint.WithX((int)(openPoint.X + this.Bounds.Width/2 - tagWindow.Width/2));
        openPoint = openPoint.WithY((int)(openPoint.Y + this.Bounds.Height/2 - tagWindow.Height/2));
        tagWindow.Position = openPoint;
        tagWindow.ShowDialog(this.Parent.Parent.Parent as Window);
        tagWindow.OpenTagWindow(tag);
        popup.Close();
      }
    }

    private void TagExclude(object sender, PointerPressedEventArgs args)
    {
      var popup = (sender as TextBlock).Parent.Parent as Popup;
      if(popup != null)
      {
        var tag = (popup.PlacementTarget as TextBlock).DataContext as Tag;
        var dc = this.DataContext as DirectoryViewModel;
        dc.ManualFilter(tag.Name, true);
        popup.Close();
      }
    }

    private void TagRemove(object sender, PointerPressedEventArgs args)
    {
      var popup = (sender as TextBlock).Parent.Parent as Popup;
      if(popup != null)
      {
        var di = (popup.PlacementTarget as TextBlock).Parent.Parent.Parent.DataContext as DirectoryItem;
        var tag = (popup.PlacementTarget as TextBlock).DataContext as Tag;
        var vm = this.DataContext as DirectoryViewModel;
        vm.RemoveTag(di, tag);
        popup.Close();
      }
    }

    private void TagHover(object sender, PointerEventArgs args)
    {
      var temp = (TextBlock)sender;
      temp.FontWeight = Avalonia.Media.FontWeight.Bold;
      args.Handled = true;
    }

    private void TagUnhover(object sender, PointerEventArgs args)
    {
      var temp = (TextBlock)sender;
      temp.FontWeight = Avalonia.Media.FontWeight.Normal;
      args.Handled = true;
    }

    private void AddTag(object sender, PointerPressedEventArgs args)
    {
      var textBlock = (TextBlock)sender;
      var vm = (DirectoryViewModel)this.DataContext;
      var textBox = (textBlock.Parent as WrapPanel).Children.First(x => x.GetType() == typeof(AutoCompleteBox)) as AutoCompleteBox;
      vm.AddTag((DirectoryItem)textBlock.DataContext, textBox.Text);
      textBox.Text = "";
      textBox.ClearValue(AutoCompleteBox.TextProperty);
      args.Handled = true;
    }

    private void AddTag(DirectoryItem item, string tag)
    {
      var vm = this.DataContext as DirectoryViewModel;
      vm.AddTag(item, tag);
    }

    private void RemoveFilter(object sender, PointerPressedEventArgs args)
    {
      var dc = this.DataContext as DirectoryViewModel;
      dc.RemoveFilter((sender as StyledElement).DataContext as Filter);
    }

    private void AddManualFilter(object sender, PointerPressedEventArgs args)
    {
      var dc = this.DataContext as DirectoryViewModel;
      dc.ManualFilter(manualFilter.Text, false);
      manualFilter.Text = "";
      manualFilter.ClearValue(AutoCompleteBox.TextProperty);
      args.Handled = true;
    }

    private void ExcludeManualFilter(object sender, PointerPressedEventArgs args)
    {
      var dc = this.DataContext as DirectoryViewModel;
      dc.ManualFilter(manualFilter.Text, true);
      manualFilter.Text = "";
      manualFilter.ClearValue(AutoCompleteBox.TextProperty);
      args.Handled = true;
    }

    private void SaveLastRowPosition(object sender, DataGridCellPointerPressedEventArgs args)
    {
      var gridBottom = ((TransformedBounds)(sender as DataGrid).TransformedBounds).Clip.Bottom;
      var cellBottom = ((TransformedBounds)args.Cell.TransformedBounds).Clip.Bottom;
      var cellHeight = ((TransformedBounds)args.Cell.TransformedBounds).Clip.Height;
      var percentageOfLastRow = (decimal)((gridBottom - cellBottom)/cellHeight);
      var numberOfRemainingRows = (int)decimal.Round(percentageOfLastRow, System.MidpointRounding.AwayFromZero);
      var itemsAsList = (sender as DataGrid).Items.Cast<DirectoryItem>().ToList();
      var currentIndex = itemsAsList.IndexOf((args.Cell.Content as IDataContextProvider).DataContext as DirectoryItem);
      if(currentIndex + numberOfRemainingRows < itemsAsList.Count)
      {
        lastObject = itemsAsList[currentIndex + numberOfRemainingRows];
        return;
      }
      lastObject = itemsAsList.Last();
    }

    private void SelectionHandler(object sender, DataGridCellPointerPressedEventArgs args)
    {
      IDataContextProvider obj = args.Cell.Content as IDataContextProvider;
      if(obj==null)
      {
        return;
      }
      var dc = obj.DataContext;
      if((obj as INamed).Name != "TagCell")
      {
        if(dc.GetType() == typeof(DirectoryItem))
        {
          switch(args.PointerPressedEventArgs.GetCurrentPoint(null).Properties.PointerUpdateKind)
          {
            case(PointerUpdateKind.LeftButtonPressed):
              SaveLastRowPosition(sender, args);
              var temp = (DirectoryItem)dc;
              if(File.Exists(temp.StartPath))
              {
                var vm = new ImageViewModel(temp.StartPath);
                var t = (MainWindowViewModel)this.Parent.DataContext;
                t.Content = vm;
              }
              break;
            case(PointerUpdateKind.RightButtonPressed):
              (this.DataContext as DirectoryViewModel).LoadBackup((DirectoryItem)dc);
              break;
          }
        }
        return;
      }
    }
  }
}