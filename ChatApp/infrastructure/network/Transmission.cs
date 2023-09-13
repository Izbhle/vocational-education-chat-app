using System.Text.Json;

namespace Network
{
    /// <summary>
    /// Transmissions are either a request, or a response to a prior request
    /// </summary>
    public enum TransmissionType
    {
        request,
        response,
        both
    }

    /// <summary>
    /// Used by the server to determine, if a request should be either relayed to another client, or processed by the server itself
    /// </summary>
    public enum TargetType
    {
        server,
        client
    }

    /// <summary>
    /// Generic Transmission type. All attributes needed by the communication Protocol are specified at the top level.
    /// </summary>
    /// <typeparam name="Req">Request</typeparam>
    /// <typeparam name="Res">Response</typeparam>
    public class Transmission<Req, Res>
    {
        public TransmissionType? transmissionType { get; set; }
        public TargetType? targetType { get; set; }
        public string? receiverId { get; set; }
        public string? senderId { get; set; }
        public Req? request { get; set; }
        public Res? response { get; set; }
    }

    /// <summary>
    /// Used to convert Transmissions between class and JSONstring format.
    /// </summary>
    /// <typeparam name="Req">Request</typeparam>
    /// <typeparam name="Res">Response</typeparam>
    class TransmissionWrapper<Req, Res>
    {
        /// <value>Attribute <c>jsonString</c>: string to transmit over network</value>
        public string jsonString;

        /// <value>Attribute <c>data</c>: class Representation for C# logic</value>
        public Transmission<Req, Res>? data;

        /// <summary>
        /// Transforms Transmission object into jsonString
        /// </summary>
        /// <param name="transmission"></param>
        public TransmissionWrapper(Transmission<Req, Res>? transmission)
        {
            data = transmission;
            jsonString = JsonSerializer.Serialize(data);
        }

        /// <summary>
        /// Parse jsonString into Transmission object and perform basic validation.
        /// Data is set to null if the jsonString is invalid
        /// </summary>
        /// <param name="json">jsonString</param>
        public TransmissionWrapper(string json)
        {
            jsonString = json;
            try
            {
                data = new Transmission<Req, Res>();
                data = JsonSerializer.Deserialize<Transmission<Req, Res>>(json);
                if (data != null) // catch basic non throwing serialization failure
                {
                    if (data.targetType == null || data.transmissionType == null) // catch invalid cases and invalidate Transmission
                    {
                        data = null;
                    }
                }
            }
            catch (JsonException)
            {
                data = null;
            }
        }
    }
}
