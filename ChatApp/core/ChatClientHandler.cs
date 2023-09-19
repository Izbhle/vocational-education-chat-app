using System.Text.Json;
using DynamicData;
using Network;

namespace ChatApp
{
    public class ChatClientHandler
    {
        public static void TransmissionHandler(
            IChatClient chatClient,
            INetworkClient<ChatRequest, ChatResponse> client,
            Transmission<ChatRequest, ChatResponse>? transmission
        )
        {
            if (transmission == null)
            {
                return;
            }
            switch (transmission.transmissionType)
            {
                case TransmissionType.request:
                    if (transmission.request == null)
                    {
                        return;
                    }
                    switch (transmission.request.requestType)
                    {
                        case ChatRequestType.Message:
                            chatClient.messagesStore.Store(transmission);
                            client.SendResponse(
                                transmission,
                                new ChatResponse
                                {
                                    requestType = ChatRequestType.Message,
                                    requestTimeId = transmission.request.requestTimeId
                                }
                            );
                            break;
                    }
                    break;
                case TransmissionType.response:
                    if (transmission.response == null)
                    {
                        return;
                    }
                    switch (transmission.response.requestType)
                    {
                        case ChatRequestType.Message:
                            chatClient.messagesStore.Store(transmission);
                            break;
                        case ChatRequestType.ClientList:
                            if (transmission.response.message == null)
                                return;
                            try
                            {
                                var clients = JsonSerializer.Deserialize<List<string>>(
                                    transmission.response.message
                                );
                                if (clients == null)
                                    return;
                                chatClient.availableClients = clients;
                            }
                            catch (JsonException) { }
                            break;
                    }
                    break;
            }
            chatClient.callback();
        }
    }
}
