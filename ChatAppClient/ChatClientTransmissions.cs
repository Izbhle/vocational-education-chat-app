using Transmissions;

namespace ChatAppClient
{
    public class ChatClientTransmissions
    {
        public static ChatRequest disconnectRequest =
            new() { requestType = ChatRequestType.DisconnectClient, };
        public static ChatRequest getClientListRequest =
            new() { requestType = ChatRequestType.ClientList, };
    }
}
