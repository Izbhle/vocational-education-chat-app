namespace ChatAppClient
{
    public interface IChatClient
    {
        public IChatRequestStore MessagesStore { get; }
        public Action Callback { get; }
        public List<string> AvailableClients { get; set; }
        public abstract void Start();
        public abstract void Dispose();
        public abstract void SendMessage(string target, string message);
        public abstract void RequestClientList();
    }
}
