public enum ChatRequestType
{
    RegisterClient,
    SendMessage
}

public enum ChatResponseError
{
    // General errors
    OperationNotSupported,
    MalformedRequest,

    // Transmission specific errors
    ReceiverUnknown,
}

public class ChatRequest
{
    public DateTime requestTimeId { get; set; }
    public ChatRequestType? requestType { get; set; }
    public string? message { get; set; }
}

public class ChatResponse
{
    public DateTime requestTimeId { get; set; }
    public ChatResponseError? error { get; set; }
    public ChatRequestType? requestType { get; set; }
    public string? message { get; set; }
}
