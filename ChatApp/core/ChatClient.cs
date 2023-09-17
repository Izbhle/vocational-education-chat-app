using Network;

namespace ChatApp
{
    public class ChatClient : IChatClient
    {
        private readonly NetworkClient<ChatRequest, ChatResponse> client;
        public IChatRequestStore receivedMessagesStore { get; }
        public IChatRequestStore sendMessagesStore { get; }

        public Action callback { get; }

        public ChatClient(string id, string ipAddress, int port, Action updateCallback)
        {
            callback = updateCallback;
            var registerRequest = new ChatRequest
            {
                requestType = ChatRequestType.RegisterClient,
                message = id
            };
            var disconnectRequest = new ChatRequest
            {
                requestType = ChatRequestType.DisconnectClient,
            };
            client = new NetworkClient<ChatRequest, ChatResponse>(
                id,
                ipAddress,
                port,
                TransmissionHandlerWrapper,
                registerRequest,
                disconnectRequest
            );
            receivedMessagesStore = new ChatRequestStore();
            sendMessagesStore = new ChatRequestStore();
            client.Start();
        }

        public void SendMessage(string target, string message)
        {
            var request = new ChatRequest
            {
                requestTimeId = DateTime.Now,
                requestType = ChatRequestType.SendMessage,
                message = message
            };
            var transmission = client.SendClientRequest(target, request);
            sendMessagesStore.Store(transmission);
        }

        private Action<Transmission<ChatRequest, ChatResponse>?> TransmissionHandlerWrapper(
            NetworkClient<ChatRequest, ChatResponse> client
        )
        {
            return (transmission) =>
            {
                ChatClientHandler.TransmissionHandler(this, client, transmission);
            };
        }
    }
}
