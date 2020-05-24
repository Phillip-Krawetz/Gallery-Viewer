using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MediaPlayer.Client.ViewModels;
using MediaPlayer.Domain.Models;

namespace MediaPlayer.Client.Views
{
  public class TagWindow : Window
  {
    private TextBox tagName;
    private ComboBox categoryList;
    public TagWindow(){}
    public TagWindow(Tag tag)
    {
      this.DataContext = new TagWindowViewModel(tag);
      InitializeComponent();
      tagName = this.FindControl<TextBox>("TagName");
      tagName.Text = tag.Name;
      categoryList = this.FindControl<ComboBox>("CategoryList");
      categoryList.SelectedIndex = tag.Category.Id - 1;
      this.FindControl<Button>("SaveButton").PropertyChanged += Save;
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    private void Save(object sender, AvaloniaPropertyChangedEventArgs args)
    {
      if(args.Property == Button.IsPressedProperty && !(bool)args.NewValue)
      {
        (this.DataContext as TagWindowViewModel).SaveTag(tagName.Text, categoryList.SelectedItem as Category);
        this.Hide();
      }
    }
  }
}