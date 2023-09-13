using Network;

namespace ChatApp
{
    public class ChatServer
    {
        private NetworkServer<ChatRequest, ChatResponse> server;

        public ChatServer()
        {
            server = new NetworkServer<ChatRequest, ChatResponse>(
                "127.0.0.1",
                1234,
                TransmissionHandlerWrapper
            );
            server.Start();
        }

        private void SendMalformedRequestResponse(
            NetworkClient<ChatRequest, ChatResponse> client,
            Transmission<ChatRequest, ChatResponse> transmission
        )
        {
            client.SendResponse(
                transmission,
                new ChatResponse { error = ChatResponseError.MalformedRequest }
            );
        }

        private void TransmissionHandler(
            NetworkServer<ChatRequest, ChatResponse> server,
            NetworkClient<ChatRequest, ChatResponse> client,
            Transmission<ChatRequest, ChatResponse>? transmission
        )
        {
            if (transmission == null)
            {
                return;
            }
            switch (transmission.targetType)
            {
                case TargetType.client:
                    if (
                        transmission.receiverId == null
                        || !server.clients.ContainsKey(transmission.receiverId)
                    )
                    {
                        client.SendResponse(
                            transmission,
                            new ChatResponse { error = ChatResponseError.ReceiverUnknown }
                        );
                        return;
                    }
                    server.clients[transmission.receiverId].SendTransmission(transmission);

                    break;
                case TargetType.server:
                    switch (transmission.transmissionType)
                    {
                        case TransmissionType.request:
                            if (transmission.request != null)
                            {
                                switch (transmission.request.requestType)
                                {
                                    case ChatRequestType.RegisterClient:
                                        if (transmission.request.message == null)
                                        {
                                            SendMalformedRequestResponse(client, transmission);
                                            return;
                                        }
                                        server.RegisterClientAction(
                                            client,
                                            transmission.request.message
                                        );
                                        break;
                                }
                            }
                            break;
                        default:
                            client.SendResponse(
                                transmission,
                                new ChatResponse { error = ChatResponseError.OperationNotSupported }
                            );
                            break;
                    }
                    break;
            }
            return;
        }

        private Func<
            NetworkClient<ChatRequest, ChatResponse>,
            Action<Transmission<ChatRequest, ChatResponse>?>
        > TransmissionHandlerWrapper(NetworkServer<ChatRequest, ChatResponse> server)
        {
            return (client) =>
            {
                return (transmission) =>
                {
                    TransmissionHandler(server, client, transmission);
                };
            };
        }
    }
}
