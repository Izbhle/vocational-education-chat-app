namespace ChatAppClient
{
    public interface IChatClient
    {
        public IChatRequestStore messagesStore { get; }
        public Action callback { get; }
        public List<string> availableClients { get; set; }
        public abstract void Start();
        public abstract void Dispose();
        public abstract void SendMessage(string target, string message);
        public abstract void RequestClientList();
    }
}