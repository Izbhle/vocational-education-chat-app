using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Views
{
    public partial class ClientWindow : Window
    {
        public ClientWindow()
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
