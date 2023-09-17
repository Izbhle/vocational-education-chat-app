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
                        case ChatRequestType.SendMessage:
                            chatClient.receivedMessagesStore.Store(transmission);
                            client.SendResponse(transmission, new ChatResponse { });
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
                        case ChatRequestType.SendMessage:
                            chatClient.sendMessagesStore.Store(transmission);
                            break;
                    }
                    break;
            }
            chatClient.callback();
        }
    }
}
