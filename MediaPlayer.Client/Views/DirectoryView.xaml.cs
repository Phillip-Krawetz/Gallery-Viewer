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
using MediaPlayer.Domain.Utilities;
using MediaPlayer.Client.Abstracts;

namespace MediaPlayer.Client.Views
{
  public class DirectoryView : AbstractUserControlWithTags
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
      var randomButton = this.FindControl<Button>("RandomButton");
      randomButton.Click += delegate
      {
        this.SelectRandom();
      };
      SearchControl = "SearchBox";
      InitializeTextBindings();
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

    private void RemoveFilter(object sender, PointerPressedEventArgs args)
    {
      var dc = this.DataContext as DirectoryViewModel;
      dc.RemoveFilter((sender as StyledElement).DataContext as Filter);
    }

    private void AddManualFilter(object sender, PointerPressedEventArgs args)
    {
      AddManualFilter();
      args.Handled = true;
    }
    private void AddManualFilter()
    {
      var dc = this.DataContext as DirectoryViewModel;
      dc.ManualFilter(manualFilter.Text, false);
      manualFilter.Text = "";
      manualFilter.ClearValue(AutoCompleteBox.TextProperty);
    }

    private void ExcludeManualFilter(object sender, PointerPressedEventArgs args)
    {
      var dc = this.DataContext as DirectoryViewModel;
      dc.ManualFilter(manualFilter.Text, true);
      manualFilter.Text = "";
      manualFilter.ClearValue(AutoCompleteBox.TextProperty);
      args.Handled = true;
    }

    private void SearchNames(object sender, PointerPressedEventArgs args)
    {
      SearchNames();
      args.Handled = true;
    }
    private void SearchNames()
    {
      var dc = this.DataContext as DirectoryViewModel;
      dc.SearchNames();
    }

    private void ConfirmPressHappened(StyledElement sender)
    {
      switch(sender.Name){
        case "SearchLabel":
          SearchNames();
          break;
        case "AddManualFilter":
          AddManualFilter();
          break;
        case "AddTagBox":
          base.ConfirmPressHappened(sender as AutoCompleteBox);
          break;
        default:
          break;
      }
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
      if((obj as INamed).Name != "TagPanel")
      {
        if(dc.GetType() == typeof(DirectoryItem))
        {
          switch(args.PointerPressedEventArgs.GetCurrentPoint(null).Properties.PointerUpdateKind)
          {
            case(PointerUpdateKind.LeftButtonPressed):
              SaveLastRowPosition(sender, args);
              var temp = (DirectoryItem)dc;
              OpenDirectory(temp);
              break;
            case(PointerUpdateKind.RightButtonPressed):
              (this.DataContext as DirectoryViewModel).LoadBackup((DirectoryItem)dc);
              break;
          }
        }
        return;
      }
    }

    private void SelectRandom()
    {
      var dc = this.DataContext as DirectoryViewModel;
      var randSel = NumberUtils.Rand.Next(dc.Items.Count);
      var selection = dc.Items[randSel];
      if(randSel != dc.Items.Count)
      {
        randSel++;
      }
      lastObject = dc.Items[randSel];
      OpenDirectory(selection);
    }

    private void OpenDirectory(DirectoryItem item)
    {
      if(File.Exists(item.StartPath))
      {
        var vm = new ImageViewModel(item);
        var t = (MainWindowViewModel)this.Parent.DataContext;
        t.Content = vm;
      }
    }

    protected override void TagLeftClick(object sender, PointerPressedEventArgs args)
    {
      var tag = (sender as StyledElement).DataContext;
      var dc = this.DataContext as DirectoryViewModel;
      dc.AddFilter(tag as Tag);
    }

    protected override void TagRightClick(object sender, PointerPressedEventArgs args)
    {
      var popup = this.FindControl<Popup>("TagPopup");
      popup.PlacementTarget = sender as Control;
      popup.Open();
    }

    protected override void TagMiddleClick(object sender, PointerPressedEventArgs args)
    {
      var tag = (sender as StyledElement).DataContext;
      var dc = this.DataContext as DirectoryViewModel;
      dc.AddFilter(tag as Tag, true);
    }

    protected override void SetSelectedDirectory(TextBlock textBlock)
    {
      (this.DataContext as DirectoryViewModel).SelectedDirectory = (DirectoryItem)textBlock.DataContext;
    }
  }
}