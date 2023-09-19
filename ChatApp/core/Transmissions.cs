using Network;

namespace ChatApp
{
    public enum ChatRequestType
    {
        RegisterClient,
        DisconnectClient,
        ClientList,
        Message
    }

    public enum ChatResponseError
    {
        // General errors
        OperationNotSupported,
        MalformedRequest,

        // Transmission specific errors
        ReceiverUnknown,
    }

    public class ChatTransmission : ITransmission<ChatRequest, ChatResponse>
    {
        public TransmissionType? transmissionType { get; set; }
        public TargetType? targetType { get; set; }
        public string? receiverId { get; set; }
        public string? senderId { get; set; }
        public ChatRequest? request { get; set; }
        public ChatResponse? response { get; set; }
    }

    public class ChatRequest
    {
        public DateTime? requestTimeId { get; set; }
        public ChatRequestType? requestType { get; set; }
        public string? message { get; set; }
    }

    public class ChatResponse
    {
        public DateTime? requestTimeId { get; set; }
        public ChatResponseError? error { get; set; }
        public ChatRequestType? requestType { get; set; }
        public string? message { get; set; }
    }
}
