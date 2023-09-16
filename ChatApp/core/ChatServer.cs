using Network;

namespace ChatApp
{
    public class ChatServer
    {
        private INetworkServer<ChatRequest, ChatResponse> server;

        public ChatServer(string ipAddress, int port)
        {
            server = new NetworkServer<ChatRequest, ChatResponse>(
                ipAddress,
                port,
                TransmissionHandlerWrapper
            );
            server.Start();
        }

        private Func<
            INetworkClient<ChatRequest, ChatResponse>,
            Action<Transmission<ChatRequest, ChatResponse>?>
        > TransmissionHandlerWrapper(INetworkServer<ChatRequest, ChatResponse> server)
        {
            return (client) =>
            {
                return (transmission) =>
                {
                    ChatServerHandler.TransmissionHandler(server, client, transmission);
                };
            };
        }
    }
}
