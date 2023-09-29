using System.Text.Json;
using Network;

namespace ChatApp
{
    public class ChatClientTransmissions
    {
        public static ChatRequest disconnectRequest =
            new() { requestType = ChatRequestType.DisconnectClient, };
        public static ChatRequest getClientListRequest =
            new() { requestType = ChatRequestType.ClientList, };
    }
}
