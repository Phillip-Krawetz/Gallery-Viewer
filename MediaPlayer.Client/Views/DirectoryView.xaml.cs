using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MediaPlayer.Domain.Models;
using MediaPlayer.Client.ViewModels;
using System.IO;

namespace MediaPlayer.Client.Views
{
  public class DirectoryView : UserControl
  {
    public DirectoryView()
    {
      InitializeComponent();
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    private void TagHandler(object sender, PointerPressedEventArgs args)
    {

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
      // var test = args.Cell.;
      // if(test != null)
      // {
      //   if(test.Name == "AddTag")
      //   {
      //     var vm = (DirectoryViewModel)this.DataContext;
      //     vm.AddTag((DirectoryItem)dc,"NewTag");
      //   }
      // }
    }
  }
}