using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MediaPlayer.Client.Views
{
    public class CategoryView : UserControl
    {
        public CategoryView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}