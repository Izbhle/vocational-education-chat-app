namespace Network
{
    /// <summary>
    /// Used by both the Server and Client applications to communicate with Transmissions.
    /// </summary>
    /// <typeparam name="Req">Request</typeparam>
    /// <typeparam name="Res">Response</typeparam>
    public interface INetworkClient<Req, Res>
    {
        /// <summary>
        /// Argument for Id can only be assigned once
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Starts the stream and the receiver thread.
        /// </summary>
        /// <param name="transmissionHandlerFactory">Transmission Handler</param>
        public void Start(
            Func<
                NetworkClient<Req, Res>,
                Action<ITransmission<Req, Res>?>
            > transmissionHandlerFactory
        );

        /// <summary>
        /// Safely closes all connections. This also puts tcpClient into an unusuable state, only use this when you mean to dispose the client
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Sends a transmission to the server
        /// </summary>
        /// <param name="transmission for the server">Transmission</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise</returns>
        public abstract bool TrySendTransmission(ITransmission<Req, Res> transmission);

        /// <summary>
        /// Send a client request to the server
        /// </summary>
        /// <param name="receiverId">ID of the target client</param>
        /// <param name="request">Request</param>
        /// <returns><c>Transmission</c> if success, <c>null</c> otherwise</returns>
        public abstract ITransmission<Req, Res>? SendClientRequest(string receiverId, Req request);

        /// <summary>
        /// Send a server request to the server
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns><c>Transmission</c> if success, <c>null</c> otherwise</returns>
        public abstract ITransmission<Req, Res>? SendServerRequest(Req request);

        /// <summary>
        /// Sends a response for a request transmission
        /// </summary>
        /// <param name="oldTransmission">Request transmission</param>
        /// <param name="response">Response to be send</param>
        /// <returns>send <c>Transmission</c> if success, <c>null</c> otherwise</returns>
        public abstract ITransmission<Req, Res>? SendResponse(
            ITransmission<Req, Res> oldTransmission,
            Res response
        );
    }
}
