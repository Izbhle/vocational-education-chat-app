using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ViewModels;
using Views;

namespace ChatApp
{
    public partial class ChatApp : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void StartupAction()
        {
            if (
                ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                && chatAppService != null
            )
            {
                chatAppService.OnExitActions.Add(() =>
                {
                    desktop.Exit -= OnExit;
                    desktop.MainWindow?.Close();
                });
                var loginWindow = desktop.MainWindow;
                desktop.MainWindow = new ChatWindow
                {
                    DataContext = new ChatWindowViewModel(chatAppService),
                };
                loginWindow?.Close();
            }
        }

        private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            chatAppService?.Exit();
        }

        private ChatAppService? chatAppService;

        public override void OnFrameworkInitializationCompleted()
        {
            chatAppService = new ChatAppService();
            chatAppService.OnStartupActions.Add(StartupAction);
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Exit += OnExit;
                desktop.MainWindow = new StartupWindow
                {
                    DataContext = new StartupWindowViewModel(chatAppService),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
