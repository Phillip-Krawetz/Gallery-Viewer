using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MediaPlayer.Client.ViewModels;
using MediaPlayer.Domain.Models;
using MediaPlayer.Storing.Repositories;

namespace MediaPlayer.Client.Views
{
  public class TagWindow : Window
  {
    private TextBox tagName;
    private ComboBox categoryList;
    private ComboBox tagList;
    private static readonly TagWindow tagWindow = new TagWindow();
    public static TagWindow GetTagWindow
    {
      get => tagWindow;
    }

    //Can't make this private but don't use it
    public TagWindow()
    {
      InitializeComponent();
      this.Hide();
    }
    public void OpenTagWindow(Tag tag)
    {
      this.DataContext = new TagWindowViewModel(tag);
      tagName = this.FindControl<TextBox>("TagName");
      tagName.Text = tag.Name;
      categoryList = this.FindControl<ComboBox>("CategoryList");
      categoryList.SelectedIndex = tag.Category.Id - 1;
      tagList = this.FindControl<ComboBox>("ParentList");
      tagList.SelectedIndex = 0;
      if(tag.ParentTag != null)
      {
        tagList.SelectedIndex = tagList.Items.Cast<Tag>().ToList<Tag>().IndexOf(tag.ParentTag);
      }
      this.FindControl<Button>("SaveButton").PropertyChanged += Save;
      this.Closing += (s, e) =>
      {
        Hide();
        DirectoryRepository.SortTags(tag);
        e.Cancel = true;
      };
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    private void Save(object sender, AvaloniaPropertyChangedEventArgs args)
    {
      if(args.Property == Button.IsPressedProperty && !(bool)args.NewValue)
      {
        (this.DataContext as TagWindowViewModel).SaveTag(tagName.Text, categoryList.SelectedItem as Category, 
              tagList.SelectedItem as Tag);
        this.Close();
      }
    }
  }
}