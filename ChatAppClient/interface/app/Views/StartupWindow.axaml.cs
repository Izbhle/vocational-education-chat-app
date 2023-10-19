using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Views
{
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Show();
        }
    }
}
