using Network;

namespace ChatApp
{
    public class ChatClient
    {
        private readonly NetworkClient<ChatRequest, ChatResponse> client;
        public readonly ChatRequestStore receivedMessagesStore;
        public readonly ChatRequestStore sendMessagesStore;

        private readonly Action callback;

        public ChatClient(string id, Action updateCallback)
        {
            callback = updateCallback;
            var registerRequest = new ChatRequest
            {
                requestType = ChatRequestType.RegisterClient,
                message = id
            };

            client = new NetworkClient<ChatRequest, ChatResponse>(
                id,
                "127.0.0.1",
                1234,
                TransmissionHandlerWrapper,
                registerRequest
            );
            receivedMessagesStore = new ChatRequestStore();
            sendMessagesStore = new ChatRequestStore();
            client.Start();
            client.SendServerRequest(
                new ChatRequest { requestType = ChatRequestType.RegisterClient, message = id }
            );
        }

        public void SendMessage(string target, string message)
        {
            var request = new ChatRequest
            {
                requestTimeId = DateTime.Now,
                requestType = ChatRequestType.SendMessage,
                message = message
            };
            var transmission = client.SendClientRequest(target, request);
            sendMessagesStore.Store(transmission);
        }

        private void TransmissionHandler(
            NetworkClient<ChatRequest, ChatResponse> client,
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
                            receivedMessagesStore.Store(transmission);
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
                            sendMessagesStore.Store(transmission);
                            break;
                    }
                    break;
            }
            callback();
        }

        private Action<Transmission<ChatRequest, ChatResponse>?> TransmissionHandlerWrapper(
            NetworkClient<ChatRequest, ChatResponse> client
        )
        {
            return (transmission) =>
            {
                TransmissionHandler(client, transmission);
            };
        }
    }
}