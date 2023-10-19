namespace Transmissions
{
    /// <summary>
    /// Generic Transmission type. All attributes needed by the communication Protocol are specified at the top level.
    /// </summary>
    /// <typeparam name="Req">Request</typeparam>
    /// <typeparam name="Res">Response</typeparam>
    public interface ITransmission<Req, Res>
    {
        public TransmissionType? transmissionType { get; set; }
        public TargetType? targetType { get; set; }
        public string? receiverId { get; set; }
        public string? senderId { get; set; }
        public Req? request { get; set; }
        public Res? response { get; set; }
    }
}
