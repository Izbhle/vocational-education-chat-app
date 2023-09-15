using Network;
using ReactiveUI;
using System;
using ChatApp;

namespace ViewModels
{
    // Instead of implementing "INotifyPropertyChanged" on our own we use "ReactiveObject" as
    // our base class. Read more about it here: https://www.reactiveui.net
    public class StartServerModel : ReactiveObject
    {
        public ChatServer? server;

        public StartServerModel()
        {
            // We can listen to any property changes with "WhenAnyValue" and do whatever we want in "Subscribe".
            this.WhenAnyValue(o => o.Name).Subscribe(o => this.RaisePropertyChanged(nameof(Name)));
        }

        public void OnClickCommand()
        {
            server = new ChatServer();
            Console.WriteLine("server started!");
            return;
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
    }
}
