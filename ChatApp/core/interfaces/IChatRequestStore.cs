using Network;

namespace ChatApp
{
    public interface IChatRequestStore
    {
        public Dictionary<
            string,
            Dictionary<DateTime, Transmission<ChatRequest, ChatResponse>>
        > requestTransmissions { get; }

        public abstract void Store(Transmission<ChatRequest, ChatResponse>? Transmission);
    }
}