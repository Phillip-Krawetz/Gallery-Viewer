using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using MediaPlayer.Client.Views;
using MediaPlayer.Domain.Models;

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

    protected void AddTag(object sender, PointerPressedEventArgs args)
    {
      var textBlock = (TextBlock)sender;
      var textBox = (textBlock.Parent as WrapPanel).Children.First(
                x => x.GetType() == typeof(AutoCompleteBox)) as AutoCompleteBox;
      SetSelectedDirectory(textBlock);
      AddTag((this.DataContext as AbstractViewModelBaseWithTags).SelectedDirectory, textBox.Text);
      textBox.Text = "";
      textBox.ClearValue(AutoCompleteBox.TextProperty);
      args.Handled = true;
    }

    protected void AddTag(DirectoryItem item, string tag)
    {
      var vm = this.DataContext as AbstractViewModelBaseWithTags;
      vm.AddTag(item, tag);
    }

    protected abstract void SetSelectedDirectory(TextBlock textBlock);

    protected void TagEdit(object sender, PointerPressedEventArgs args)
    {
      var popup = this.FindControl<Popup>("TagPopup");
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

    protected void ConfirmPressHappened(AutoCompleteBox sender)
    {
      AddTag(sender.DataContext as DirectoryItem, sender.Text);
      sender.Text = "";
      sender.ClearValue(AutoCompleteBox.TextProperty);
    }
  }
}