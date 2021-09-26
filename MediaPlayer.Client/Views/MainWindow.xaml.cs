using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MediaPlayer.Client.ViewModels;
using MediaPlayer.Domain.Variables;

namespace MediaPlayer.Client.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var backButton = this.Find<Button>("BackButton");
            backButton.Click += delegate
            {
              this.BackButtonPress();
            };
            this.DataContextChanged += AssignKeyBindings;

            var categoryButton = this.Find<Button>("CategoryButton");
            categoryButton.Click += delegate
            {
              (this.DataContext as MainWindowViewModel).Content = new CategoryViewModel();
            };

            var optionsButton = this.Find<Button>("OptionsButton");
            optionsButton.Click += delegate
            {
              (this.DataContext as MainWindowViewModel).Content = new OptionsViewModel();
            };

            mainContent = this.Find<UserControl>("MainContent");
            menuBar = this.Find<WrapPanel>("MenuBar");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void AssignKeyBindings(object sender, System.EventArgs args)
        {
          var vm = this.DataContext as MainWindowViewModel;
          vm.BackEvent += delegate
          {
            this.BackButtonPress();
          };
        }

        private void BackButtonPress()
        {
          if(currentPage > 0){
            var t = (MainWindowViewModel)this.DataContext;
            oldContent.Remove(OldContent[currentPage]);
            currentPage--;
            t.Content = (ViewModelBase)OldContent[currentPage];
            this.Title = (mainContent.Content as ViewModelBase).MainTitle;
          }
        }

        private UserControl mainContent;

        private WrapPanel menuBar;

        private static int currentPage = -1;

        private static List<object> oldContent = new List<object>();

        public static List<object> OldContent
        {
          get => oldContent;
        }

        private void OpenMenuBar(object sender, PointerEventArgs args)
        {
          var obj = sender as WrapPanel;
          if(obj != null)
          {
            obj.Height = 30;
            obj.ItemHeight = 30;
          }
          args.Handled = true;
        }

        private void CloseMenuBar(object sender, PointerEventArgs args)
        {
          var obj = sender as WrapPanel;
          if(obj != null)
          {
            obj.ItemHeight = 10;
            obj.Height = 10;
          }
          args.Handled = true;
        }

        public void ContentChangeHandler(object sender, AvaloniaPropertyChangedEventArgs e)
        {
          if(e.Property.Name == "Content")
          {
            if(!oldContent.Contains(mainContent.Content))
            {
              currentPage++;
              oldContent.Add(mainContent.Content);
              this.Title = (mainContent.Content as ViewModelBase).MainTitle;
            }
            this.Focus();
          }
        }
    }
}