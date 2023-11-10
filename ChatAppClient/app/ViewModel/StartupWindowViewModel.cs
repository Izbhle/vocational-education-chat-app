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
        private readonly ChatAppModel chatAppModel;

        public void Login()
        {
            if (Name == null)
            {
                return;
            }
            if (IsLaunchServer)
            {
                chatAppModel.Server = ChatServer.CreateNew(IpAddress, 1234);
                chatAppModel.Server.Start();
            }
            chatAppModel.Name = Name;
            Thread.Sleep(200);
            chatAppModel.Client = ChatClient.CreateNew(
                Name,
                IpAddress,
                1234,
                () => chatAppModel.OnTransmissionCallback?.Invoke(),
                (ChatLogType type, string message) =>
                    chatAppModel.OnLogCallback?.Invoke(type, message)
            );
            chatAppModel.Client.Start();
            chatAppModel.OnStartupCallback?.Invoke();
        }

        public StartupWindowViewModel(ChatAppModel model)
        {
            IpAddress = "127.0.0.1";
            Port = "1234";
            Name = model.Name;
            IsLaunchServer = false;
            chatAppModel = model;
        }
    }
}
