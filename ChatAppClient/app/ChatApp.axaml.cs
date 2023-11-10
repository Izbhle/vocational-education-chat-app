using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ViewModels;
using Views;

namespace ChatAppClient
{
    public partial class ChatApp : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void StartupAction()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var loginWindow = desktop.MainWindow;
                desktop.MainWindow = new ChatWindow
                {
                    DataContext = new ChatWindowViewModel(chatAppModel),
                };
                chatAppModel.OnExitCallback += desktop.MainWindow.Close;
                loginWindow?.Close();
            }
        }

        private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            chatAppModel?.Dispose();
        }

        private readonly ChatAppModel chatAppModel = new();

        public override void OnFrameworkInitializationCompleted()
        {
            chatAppModel.OnStartupCallback += StartupAction;
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Exit += OnExit;
                desktop.MainWindow = new StartupWindow
                {
                    DataContext = new StartupWindowViewModel(chatAppModel),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
