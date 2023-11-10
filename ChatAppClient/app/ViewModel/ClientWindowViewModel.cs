using ChatAppClient;
using ReactiveUI;
using Transmissions;

namespace ViewModels
{
    public class ChatWindowViewModel : ViewModelBase
    {
        public string Name { get; }
        private readonly ChatAppModel chatAppModel;

        public List<string> AvailableClients
        {
            get { return chatAppModel.Client?.AvailableClients ?? new List<string>(); }
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
            if (chatAppModel.Client == null)
            {
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(Target) && !string.IsNullOrEmpty(Message))
                {
                    chatAppModel.Client.SendMessage(Target, Message);
                }
            }
        }

        public void RefreshClientList()
        {
            if (chatAppModel.Client == null)
            {
                return;
            }
            chatAppModel.Client.RequestClientList();
        }

        public List<ChatTransmission> Messages
        {
            get
            {
                if (chatAppModel.Client == null)
                {
                    return new List<ChatTransmission>();
                }
                if (
                    Target != null
                    && chatAppModel.Client.MessagesStore.RequestTransmissions.ContainsKey(Target)
                )
                {
                    return chatAppModel.Client.MessagesStore.RequestTransmissions[
                        Target
                    ].Values.ToList();
                }
                return new List<ChatTransmission>();
            }
        }

        public void Exit()
        {
            chatAppModel.OnExitCallback?.Invoke();
        }

        public ChatWindowViewModel(ChatAppModel model)
        {
            chatAppModel = model;
            chatAppModel.OnTransmissionCallback += () =>
                this.RaisePropertyChanged(nameof(Messages));
            chatAppModel.OnTransmissionCallback += () =>
                this.RaisePropertyChanged(nameof(AvailableClients));
            Name = model.Name;
        }
    }
}
