using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Views
{
    public partial class ChatWindow : Window
    {
        public ChatWindow()
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
