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
        /// Use this to get all valid clients.
        /// </summary>
        /// <returns>A list with the ids of all connected clients</returns>
        public abstract List<string> GetListOfClientIds();

        /// <summary>
        /// Used to get a specific client
        /// </summary>
        /// <param name="id">Id of the client</param>
        /// <returns>The requested client if it is connected, null otherwise</returns>
        public abstract INetworkClient<Req, Res>? GetClient(string? id);

        /// <summary>
        /// Method that sends a response to a all connected clients
        /// </summary>
        /// <param name="transmission">Transmission to be sent</param>
        public abstract void SendResponseToAllClients(Res response);

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
