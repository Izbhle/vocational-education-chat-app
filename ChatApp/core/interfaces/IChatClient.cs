using Network;

namespace ChatApp
{
    public interface IChatClient
    {
        public IChatRequestStore receivedMessagesStore { get; }
        public IChatRequestStore sendMessagesStore { get; }
        public Action callback { get; }

        public abstract void SendMessage(string target, string message);
    }
}
