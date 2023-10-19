namespace Transmissions
{
    public class ChatTransmission : ITransmission<ChatRequest, ChatResponse>
    {
        public TransmissionType? transmissionType { get; set; }
        public TargetType? targetType { get; set; }
        public string? receiverId { get; set; }
        public string? senderId { get; set; }
        public ChatRequest? request { get; set; }
        public ChatResponse? response { get; set; }
    }
}
