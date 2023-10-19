namespace Transmissions
{
    public enum ChatResponseError
    {
        // General errors
        OperationNotSupported,
        MalformedRequest,

        // Transmission specific errors
        ReceiverUnknown,
    }
}
