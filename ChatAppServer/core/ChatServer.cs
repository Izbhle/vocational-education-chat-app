using System.Text.Json;
using Network;
using Transmissions;

namespace ChatAppServer
{
    public class ChatServer : IChatServer
    {
        private INetworkServer<ChatRequest, ChatResponse> server;

        public static ChatServer CreateNew(string ipAddress, int port)
        {
            var server = new NetworkServer<ChatRequest, ChatResponse>(ipAddress, port);
            return new ChatServer(server);
        }

        public ChatServer(INetworkServer<ChatRequest, ChatResponse> networkServer)
        {
            server = networkServer;
        }

        public void Start()
        {
            server.Start(TransmissionHandlerWrapper);
        }

        public void Dispose()
        {
            server.Dispose();
        }

        public ChatResponse CreateListOfClientsResponse()
        {
            List<string> clients = server.GetListOfClientIds();
            string message = JsonSerializer.Serialize(clients);
            return new ChatResponse { requestType = ChatRequestType.ClientList, message = message };
        }

        private Func<
            INetworkClient<ChatRequest, ChatResponse>,
            Action<ITransmission<ChatRequest, ChatResponse>?>
        > TransmissionHandlerWrapper(INetworkServer<ChatRequest, ChatResponse> server)
        {
            return (client) =>
            {
                return (transmission) =>
                {
                    ChatServerHandler.TransmissionHandler(this, server, client, transmission);
                };
            };
        }
    }
}
