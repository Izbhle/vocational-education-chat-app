using System.Collections.ObjectModel;
using Avalonia.Controls;
using ChatApp;
using ReactiveUI;

namespace ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public bool IsLaunchServer { get; set; }
        public string Name { get; set; }
        public ChatClient? client;
        public ChatServer? server;

        private bool _IsLoginVisible = true;

        public bool IsLoginVisible
        {
            get => _IsLoginVisible;
            set => this.RaiseAndSetIfChanged(ref _IsLoginVisible, value);
        }
        private bool _IsMainVisible = true;

        public bool IsMainVisible
        {
            get => _IsMainVisible;
            set => this.RaiseAndSetIfChanged(ref _IsMainVisible, value);
        }

        public List<string> AvailableClients
        {
            get { return client?.availableClients ?? new List<string>(); }
        }

        public void Login()
        {
            if (Name == null)
            {
                return;
            }
            if (IsLaunchServer)
            {
                server = new ChatServer(IpAddress, 1234);
            }
            Thread.Sleep(1000);
            client = new ChatClient(
                Name,
                IpAddress,
                1234,
                () =>
                {
                    this.RaisePropertyChanged(nameof(Messages));
                    this.RaisePropertyChanged(nameof(AvailableClients));
                }
            );
            IsMainVisible = true;
            IsLoginVisible = false;
        }

        private string? _Target; // This is our backing field for Name

        public string? Target
        {
            get { return _Target; }
            set
            {
                // We can use "RaiseAndSetIfChanged" to check if the value changed and automatically notify the UI
                this.RaiseAndSetIfChanged(ref _Target, value);
                this.RaisePropertyChanged(nameof(Messages));
            }
        }
        private string? _Message; // This is our backing field for Name

        public string? Message
        {
            get { return _Message; }
            set
            {
                // We can use "RaiseAndSetIfChanged" to check if the value changed and automatically notify the UI
                this.RaiseAndSetIfChanged(ref _Message, value);
            }
        }

        public void Send()
        {
            if (client == null)
            {
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(Target) && !string.IsNullOrEmpty(Message))
                {
                    client.SendMessage(Target, Message);
                }
            }
        }

        public void RefreshClientList()
        {
            if (client == null)
            {
                return;
            }
            client.RequestClientList();
        }

        public string Messages
        {
            get
            {
                if (client == null)
                {
                    return "Client Disconnected";
                }
                string result = "";
                if (Target != null && client.messagesStore.requestTransmissions.ContainsKey(Target))
                {
                    client.messagesStore.requestTransmissions[Target]
                        ?.ToList()
                        .ForEach(
                            (transmission) =>
                            {
                                result +=
                                    "from "
                                        + transmission.Value.senderId
                                        + ": "
                                        + transmission.Value.request?.message
                                    ?? " ";
                            }
                        );
                }
                return result;
            }
        }

        public MainWindowViewModel()
        {
            IpAddress = "127.0.0.1";
            Port = "1234";
            IsLaunchServer = true;
            Name = "test";
            IsLoginVisible = true;
            IsMainVisible = false;
        }
    }
}
