using ChatAppClient;
using ReactiveUI;
using Transmissions;

namespace ViewModels
{
    public class ChatWindowViewModel : ViewModelBase
    {
        public string Name { get; }
        public readonly ChatAppService services;

        public List<string> AvailableClients
        {
            get { return services.Client?.availableClients ?? new List<string>(); }
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
            if (services.Client == null)
            {
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(Target) && !string.IsNullOrEmpty(Message))
                {
                    services.Client.SendMessage(Target, Message);
                }
            }
        }

        public void RefreshClientList()
        {
            if (services.Client == null)
            {
                return;
            }
            services.Client.RequestClientList();
        }

        public List<ChatTransmission> Messages
        {
            get
            {
                if (services.Client == null)
                {
                    return new List<ChatTransmission>();
                }
                if (
                    Target != null
                    && services.Client.messagesStore.requestTransmissions.ContainsKey(Target)
                )
                {
                    return services.Client.messagesStore.requestTransmissions[
                        Target
                    ].Values.ToList();
                }
                return new List<ChatTransmission>();
            }
        }

        public void Exit()
        {
            services.Exit();
        }

        public ChatWindowViewModel(ChatAppService service)
        {
            services = service;
            services.OnTransmissionActions.Add(() => this.RaisePropertyChanged(nameof(Messages)));
            services.OnTransmissionActions.Add(
                () => this.RaisePropertyChanged(nameof(AvailableClients))
            );
            Name = service.Name;
        }
    }
}
