using Network;
using ReactiveUI;
using System;
using ChatApp;

namespace ViewModels
{
    // Instead of implementing "INotifyPropertyChanged" on our own we use "ReactiveObject" as
    // our base class. Read more about it here: https://www.reactiveui.net
    public class StartClientModel : ReactiveObject
    {
        public ChatClient? client;
        public StartClientModel()
        {
            // We can listen to any property changes with "WhenAnyValue" and do whatever we want in "Subscribe".
            this.WhenAnyValue(o => o.Target)
                .Subscribe(o =>
                {
                    this.RaisePropertyChanged(nameof(Messages));
                    this.RaisePropertyChanged(nameof(Target));
                }
            );
        }
        public void OnClickCommand()
        {
            if (client == null)
            {
                if (Name != null)
                {
                    client = new ChatClient(Name, () => this.RaisePropertyChanged(nameof(Messages)));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Target) && !string.IsNullOrEmpty(Message))
                {
                    client.SendMessage(Target, Message);
                }
            }
        }
        public bool updater;
        public string Messages
        {
            get
            {
                if (client == null)
                {
                    return "Client Disconnected";
                }
                string result = "";
                if (Target != null && client.sendMessagesStore.requestTransmissions.ContainsKey(Target))
                {
                    client.sendMessagesStore.requestTransmissions[Target]?.ToList().ForEach((transmission) =>
                    {
                        result += "from " + transmission.Value.senderId + ": " + transmission.Value.request?.message ?? " ";
                    });
                }
                return result;
            }
        }

        private string? _Name; // This is our backing field for Name

        public string? Name
        {
            get { return _Name; }
            set
            {
                // We can use "RaiseAndSetIfChanged" to check if the value changed and automatically notify the UI
                this.RaiseAndSetIfChanged(ref _Name, value);
            }
        }
        private string? _Message; // This is our backing field for Message

        public string? Message
        {
            get { return _Message; }
            set
            {
                // We can use "RaiseAndSetIfChanged" to check if the value changed and automatically notify the UI
                this.RaiseAndSetIfChanged(ref _Message, value);
            }
        }
        private string? _Target; // This is our backing field for Name

        public string? Target
        {
            get { return _Target; }
            set
            {
                // We can use "RaiseAndSetIfChanged" to check if the value changed and automatically notify the UI
                this.RaiseAndSetIfChanged(ref _Target, value);
            }
        }
    }
}
