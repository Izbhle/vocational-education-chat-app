using System.Text.Json;
using Network;
using Transmissions;

namespace ChatAppClient
{
    public class ChatClientHandler
    {
        public static void TransmissionHandler(
            IChatClient chatClient,
            INetworkClient<ChatRequest, ChatResponse> client,
            ITransmission<ChatRequest, ChatResponse>? transmission
        )
        {
            if (transmission == null)
            {
                chatClient.LogCallback.Invoke(ChatLogType.warning, "Received empty Transmission");
                return;
            }
            switch (transmission.transmissionType)
            {
                case TransmissionType.request:
                    if (transmission.request == null)
                    {
                        chatClient.LogCallback.Invoke(
                            ChatLogType.warning,
                            "Received empty Request"
                        );
                        break;
                    }
                    switch (transmission.request.requestType)
                    {
                        case ChatRequestType.Message:
                            chatClient.MessagesStore.Store(transmission);
                            client.SendResponse(
                                transmission,
                                new ChatResponse
                                {
                                    requestType = ChatRequestType.Message,
                                    requestTimeId = transmission.request.requestTimeId
                                }
                            );
                            chatClient.LogCallback.Invoke(ChatLogType.info, "Message received");
                            break;
                    }
                    break;
                case TransmissionType.response:
                    if (transmission.response == null)
                    {
                        chatClient.LogCallback.Invoke(
                            ChatLogType.warning,
                            "Received empty Response"
                        );
                        break;
                    }
                    switch (transmission.response.requestType)
                    {
                        case ChatRequestType.Message:
                            chatClient.MessagesStore.Store(transmission);
                            chatClient.LogCallback.Invoke(
                                ChatLogType.info,
                                "Message send successfully"
                            );
                            break;
                        case ChatRequestType.ClientList:
                            if (transmission.response.message == null)
                            {
                                chatClient.LogCallback.Invoke(
                                    ChatLogType.warning,
                                    "Received Response with empty message"
                                );
                                break;
                            }
                            try
                            {
                                var clients = JsonSerializer.Deserialize<List<string>>(
                                    transmission.response.message
                                );
                                if (clients == null)
                                {
                                    chatClient.LogCallback.Invoke(
                                        ChatLogType.warning,
                                        "Received invalid list of clients"
                                    );
                                    break;
                                }
                                chatClient.AvailableClients = clients;
                            }
                            catch (JsonException)
                            {
                                chatClient.LogCallback.Invoke(
                                    ChatLogType.warning,
                                    "Received invalid list of clients"
                                );
                            }
                            break;
                    }
                    break;
            }
            chatClient.Callback();
        }
    }
}
