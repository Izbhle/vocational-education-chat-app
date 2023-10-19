using Network;
using Transmissions;

namespace ChatAppClient
{
    public class ChatClient : IChatClient
    {
        private readonly INetworkClient<ChatRequest, ChatResponse> client;
        public IChatRequestStore messagesStore { get; }
        public List<string> availableClients { get; set; }

        public Action callback { get; }

        public static ChatClient CreateNew(string id, string ipAddress, int port, Action callback)
        {
            var registerRequest = new ChatRequest
            {
                requestType = ChatRequestType.RegisterClient,
                message = id
            };
            var client = new NetworkClient<ChatRequest, ChatResponse>(
                id,
                ipAddress,
                port,
                registerRequest,
                ChatClientTransmissions.disconnectRequest
            );
            return new ChatClient(id, client, callback);
        }

        public ChatClient(
            string id,
            INetworkClient<ChatRequest, ChatResponse> networkClient,
            Action updateCallback
        )
        {
            availableClients = new List<string>();
            callback = updateCallback;
            client = networkClient;
            messagesStore = new ChatRequestStore(id);
        }

        public void Start()
        {
            client.Start(TransmissionHandlerWrapper);
        }

        public void Dispose()
        {
            client.Dispose();
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
            client.SendServerRequest(ChatClientTransmissions.getClientListRequest);
        }

        private Action<ITransmission<ChatRequest, ChatResponse>?> TransmissionHandlerWrapper(
            INetworkClient<ChatRequest, ChatResponse> client
        )
        {
            return (transmission) =>
            {
                ChatClientHandler.TransmissionHandler(this, client, transmission);
            };
        }
    }
}
