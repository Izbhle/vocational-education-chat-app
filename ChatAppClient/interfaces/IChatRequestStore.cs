using Transmissions;

namespace ChatAppClient
{
    public interface IChatRequestStore
    {
        public Dictionary<
            string,
            Dictionary<string, ChatTransmission>
        > RequestTransmissions { get; }

        public abstract void Store(ITransmission<ChatRequest, ChatResponse>? transmission);
    }
}
