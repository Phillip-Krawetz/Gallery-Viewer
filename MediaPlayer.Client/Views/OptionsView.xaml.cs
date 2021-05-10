using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MediaPlayer.Client.Converters;
using MediaPlayer.Client.ViewModels;
using MediaPlayer.Domain.Utilities;
using MediaPlayer.Domain.Variables;

namespace MediaPlayer.Client.Views
{
  public class OptionsView : UserControl
  {
    private Grid mainPanel;
    public OptionsView()
    {
      InitializeComponent();
      mainPanel = this.FindControl<Grid>("MainPanel");
      mainPanel.PropertyChanged += FillOptions;
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    private void FillOptions(object sender, AvaloniaPropertyChangedEventArgs args)
    {
      if(args.Property == Grid.BoundsProperty)
      {
        mainPanel.PropertyChanged -= FillOptions;
        var typeDict = new Dictionary<Type, int>()
        {
          {typeof(bool), 1},
          {typeof(KeyGesture), 2}
        };
        var optionsPanel = this.FindControl<StackPanel>("OptionsPanel");
        foreach(var item in InitialOptions.Properties)
        {
          var newRow = new WrapPanel();
          optionsPanel.Children.Add(newRow);
          newRow.Children.Add(new TextBlock(){Text = StringUtils.SplitCamelCase(item.Name)});
          var itemValue = item.GetValue((this.DataContext as OptionsViewModel).CurrentOptions, null);
          switch(typeDict.GetValueOrDefault(item.PropertyType))
          {
            case 1:
              var checkBox = new CheckBox();
              newRow.Children.Add(checkBox);
              checkBox.Bind(CheckBox.IsCheckedProperty, new Binding("CurrentOptions."+item.Name));
              break;
            case 2:
              var propertyText = new TextBox();
              newRow.Children.Add(propertyText);
              propertyText.Bind(TextBox.TextProperty, 
                  new Binding("CurrentOptions."+item.Name){Converter = new KeyGestureConverter()});
              break;
            default:
              System.Console.WriteLine("none of the above");
              break;
          }
          System.Console.WriteLine(item.GetValue((this.DataContext as OptionsViewModel).CurrentOptions, null));
        }
      }
    }
  }
}