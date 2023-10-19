namespace Transmissions
{
    public class ChatResponse
    {
        public DateTime? requestTimeId { get; set; }
        public ChatResponseError? error { get; set; }
        public ChatRequestType? requestType { get; set; }
        public string? message { get; set; }
    }
}
