using Avalonia.Controls;
using Avalonia.Markup.Xaml;


namespace Views
{
    public partial class SetupWindow : Window
    {
        public SetupWindow()
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
