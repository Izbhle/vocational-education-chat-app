using Network;

namespace ChatApp
{
    public class ChatServerHandler
    {
        public static readonly ChatResponse malformedRequestResponse =
            new() { error = ChatResponseError.MalformedRequest };

        public static readonly ChatResponse operationNotSupportedResponse =
            new() { error = ChatResponseError.OperationNotSupported };

        public static void TransmissionHandler(
            INetworkServer<ChatRequest, ChatResponse> server,
            INetworkClient<ChatRequest, ChatResponse> client,
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
                    bool success = server.TrySendTransmission(
                        transmission.receiverId,
                        transmission
                    );
                    if (!success)
                    {
                        client.SendResponse(transmission, malformedRequestResponse);
                        return;
                    }
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
                                            client.SendResponse(
                                                transmission,
                                                malformedRequestResponse
                                            );
                                            return;
                                        }
                                        server.RegisterClientAction(
                                            client,
                                            transmission.request.message
                                        );
                                        break;
                                    case ChatRequestType.DisconnectClient:
                                        server.DisconnectClientAction(client.Id);
                                        client.Dispose();
                                        break;
                                }
                            }
                            break;
                        default:
                            client.SendResponse(transmission, operationNotSupportedResponse);
                            break;
                    }
                    break;
            }
            return;
        }
    }
}
