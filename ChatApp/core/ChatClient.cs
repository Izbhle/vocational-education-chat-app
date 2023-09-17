using Network;

namespace ChatApp
{
    public class ChatClient : IChatClient
    {
        private readonly NetworkClient<ChatRequest, ChatResponse> client;
        public IChatRequestStore messagesStore { get; }
        public List<string> availableClients { get; set; }

        public Action callback { get; }

        public ChatClient(string id, string ipAddress, int port, Action updateCallback)
        {
            availableClients = new List<string>();
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
            messagesStore = new ChatRequestStore(id);
            client.Start();
        }

        public void SendMessage(string target, string message)
        {
            var request = new ChatRequest
            {
                requestTimeId = DateTime.Now,
                requestType = ChatRequestType.Message,
                message = message
            };
            var transmission = client.SendClientRequest(target, request);
            messagesStore.Store(transmission);
        }

        public void RequestClientList()
        {
            var request = new ChatRequest { requestType = ChatRequestType.ClientList, };
            client.SendServerRequest(request);
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
