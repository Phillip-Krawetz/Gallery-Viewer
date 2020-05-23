using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MediaPlayer.Client.ViewModels;
using MediaPlayer.Domain.Models;

namespace MediaPlayer.Client.Views
{
  public class CategoryView : UserControl
  {
    private TextBox nameField;
    private TextBox rField;
    private TextBox gField;
    private TextBox bField;
    private Button saveButton;
    private Category currentCategory;
    public CategoryView()
    {
      InitializeComponent();
      nameField = this.FindControl<TextBox>("CategoryNameBox");
      rField = this.FindControl<TextBox>("CategoryRBox");
      gField = this.FindControl<TextBox>("CategoryGBox");
      bField = this.FindControl<TextBox>("CategoryBBox");
      saveButton = this.FindControl<Button>("SaveButton");
      saveButton.PropertyChanged += UpdateCategory;
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    private void AddCategory(object sender, PointerPressedEventArgs args)
    {
      currentCategory = new Category(){ Name = "New" };
      SetFields();
    }

    private void CategorySelection(object sender, PointerPressedEventArgs args)
    {
      currentCategory = (sender as TextBlock).DataContext as Category;
      SetFields();
    }

    private void SetFields()
    {
      nameField.Text = currentCategory.Name;
      rField.Text = currentCategory.R.ToString();
      gField.Text = currentCategory.G.ToString();
      bField.Text = currentCategory.B.ToString();
    }

    private void UpdateCategory(object sender, AvaloniaPropertyChangedEventArgs args)
    {
      if(args.Property == Button.IsPressedProperty && !(bool)args.NewValue)
      {
        currentCategory.Name = nameField.Text;
        currentCategory.R = Convert.ToByte(rField.Text);
        currentCategory.G = Convert.ToByte(gField.Text);
        currentCategory.B = Convert.ToByte(bField.Text);
        var vm = this.DataContext as CategoryViewModel;
        vm.UpdateCategory(currentCategory);
      }
    }
  }
}