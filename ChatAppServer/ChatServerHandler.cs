using Network;
using Transmissions;

namespace ChatAppServer
{
    public class ChatServerHandler
    {
        public static readonly ChatResponse malformedRequestResponse =
            new() { error = ChatResponseError.MalformedRequest };

        public static readonly ChatResponse operationNotSupportedResponse =
            new() { error = ChatResponseError.OperationNotSupported };

        public static readonly ChatResponse receiverUnknownResponse =
            new() { error = ChatResponseError.ReceiverUnknown };

        public static void TransmissionHandler(
            IChatServer chatServer,
            INetworkServer<ChatRequest, ChatResponse> server,
            INetworkClient<ChatRequest, ChatResponse> client,
            ITransmission<ChatRequest, ChatResponse>? transmission
        )
        {
            if (transmission == null)
            {
                return;
            }
            switch (transmission.targetType)
            {
                case TargetType.client:
                    var serverClient = server.GetClient(transmission.receiverId);
                    if (serverClient == null)
                    {
                        client.SendResponse(transmission, receiverUnknownResponse);
                        break;
                    }
                    bool success = serverClient.TrySendTransmission(transmission);
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
                                            break;
                                        }
                                        server.RegisterClientAction(
                                            client,
                                            transmission.request.message
                                        );
                                        server.SendResponseToAllClients(
                                            chatServer.CreateListOfClientsResponse()
                                        );
                                        break;
                                    case ChatRequestType.DisconnectClient:
                                        client.Dispose();
                                        Thread.Sleep(200);
                                        server.DisconnectClientAction(client.Id);
                                        server.SendResponseToAllClients(
                                            chatServer.CreateListOfClientsResponse()
                                        );
                                        break;
                                    case ChatRequestType.ClientList:
                                        client.SendResponse(
                                            transmission,
                                            chatServer.CreateListOfClientsResponse()
                                        );
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
