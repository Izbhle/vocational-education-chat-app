using ChatAppClient;
using ChatAppServer;

namespace ViewModels
{
    public class StartupWindowViewModel : ViewModelBase
    {
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public bool IsLaunchServer { get; set; }
        public string Name { get; set; }
        public readonly ChatAppService services;

        public void Login()
        {
            if (Name == null)
            {
                return;
            }
            if (IsLaunchServer)
            {
                services.Server = ChatServer.CreateNew(IpAddress, 1234);
                services.Server.Start();
            }
            services.Name = Name;
            Thread.Sleep(1000);
            services.Client = ChatClient.CreateNew(
                Name,
                IpAddress,
                1234,
                services.RunOnTransmissionActions
            );
            services.Client.Start();
            services.RunOnStartupActions();
        }

        public StartupWindowViewModel(ChatAppService service)
        {
            IpAddress = "127.0.0.1";
            Port = "1234";
            Name = service.Name;
            IsLaunchServer = true;
            services = service;
        }
    }
}
