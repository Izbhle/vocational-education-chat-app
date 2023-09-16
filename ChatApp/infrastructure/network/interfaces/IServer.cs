using System.Net;
using System.Net.Sockets;

namespace Network
{
    /// <summary>
    /// Allows incoming clients to connect. Provides network communication via Transmissions.
    /// </summary>
    /// <typeparam name="Req">Request</typeparam>
    /// <typeparam name="Res">Response</typeparam>
    public interface INetworkServer<Req, Res>
    {
        /// <summary>
        /// Reference all clients by clientId. Used to Relay Transmissions.
        /// </summary>
        public Dictionary<string, INetworkClient<Req, Res>> clients { get; }

        /// <summary>
        /// Method that tries to send a transmission to a connected client
        /// </summary>
        /// <param name="targetId">Id of target client</param>
        /// <param name="transmission">Transmission to be sent</param>
        public abstract bool TrySendTransmission(
            string? targetId,
            Transmission<Req, Res> transmission
        );

        /// <summary>
        /// /// Starts the TcpListener to start accepting incoming connections
        /// </summary>
        public abstract void RegisterClientAction(INetworkClient<Req, Res> client, string id);

        /// <summary>
        /// Provides means to manually disconnect a client
        /// </summary>
        /// <param name="id"></param>
        public abstract void DisconnectClientAction(string? id);

        /// <summary>
        /// /// Starts the TcpListener to start accepting incoming connections
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Disposes the Server, safely disconnecting all clients.
        /// </summary>
        public abstract void Dispose();
    }
}
