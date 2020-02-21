using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MediaPlayer.Client.ViewModels;

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
            mainContent = this.Find<UserControl>("MainContent");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void BackButtonPress()
        {
          if(currentPage > 0){
            var t = (MainWindowViewModel)this.DataContext;
            oldContent.Remove(OldContent[currentPage]);
            currentPage--;
            t.Content = (ViewModelBase)OldContent[currentPage];
          }
        }

        private UserControl mainContent;

        private static int currentPage = -1;

        private static List<object> oldContent = new List<object>();

        public static List<object> OldContent
        {
          get => oldContent;
        }

        public void ContentChangeHandler(object sender, AvaloniaPropertyChangedEventArgs e)
        {
          if(e.Property.Name == "Content")
          {
            if(!oldContent.Contains(mainContent.Content))
            {
              currentPage++;
              oldContent.Add(mainContent.Content);
            }
            this.Focus();
          }
        }
    }
}