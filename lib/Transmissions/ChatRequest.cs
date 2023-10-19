namespace Transmissions
{
    public class ChatRequest
    {
        public DateTime? requestTimeId { get; set; }
        public ChatRequestType? requestType { get; set; }
        public string? message { get; set; }
    }
}
