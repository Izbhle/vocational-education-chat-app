using Network;
using Transmissions;

namespace ChatAppClient
{
    public class ChatClient : IChatClient
    {
        private readonly INetworkClient<ChatRequest, ChatResponse> client;
        public IChatRequestStore MessagesStore { get; }
        public List<string> AvailableClients { get; set; }
        public Action Callback { get; }
        public Action<ChatLogType, string> LogCallback { get; }

        public static ChatClient CreateNew(
            string id,
            string ipAddress,
            int port,
            Action callback,
            Action<ChatLogType, string> logCallback
        )
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
            return new ChatClient(id, client, callback, logCallback);
        }

        public ChatClient(
            string id,
            INetworkClient<ChatRequest, ChatResponse> networkClient,
            Action updateCallback,
            Action<ChatLogType, string> logCallback
        )
        {
            AvailableClients = new List<string>();
            Callback = updateCallback;
            LogCallback = logCallback;
            client = networkClient;
            MessagesStore = new ChatRequestStore(id);
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
            MessagesStore.Store(transmission);
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
