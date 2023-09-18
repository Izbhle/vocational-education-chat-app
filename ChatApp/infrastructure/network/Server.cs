using System.Net;
using System.Net.Sockets;

namespace Network
{
    /// <summary>
    /// Allows incoming clients to connect. Provides network communication via Transmissions.
    /// </summary>
    /// <typeparam name="Req">Request</typeparam>
    /// <typeparam name="Res">Response</typeparam>
    public class NetworkServer<Req, Res> : INetworkServer<Req, Res>
    {
        /// <summary>
        /// Reference all clients by clientId. Used to Relay Transmissions.
        /// </summary>
        public Dictionary<string, INetworkClient<Req, Res>> clients { get; }
        private readonly ClientConnectionListener clientConnectionListener;

        /// <summary>
        /// Factory function used to inject the NetworkClient object into the Handler Bus
        /// </summary>
        /// <returns>Transmission handler provided by the Application</returns>
        private readonly Func<
            INetworkClient<Req, Res>,
            Action<Transmission<Req, Res>?>
        > transmissionHandlerClientFactory;

        /// <summary>
        ///
        /// </summary>
        /// <param name="ipAddress">IP Address of the NetworkServer</param>
        /// <param name="port">Port of the NetworkServer</param>
        /// <param name="transmissionHandlerServerFactory">Factory function used to inject the NetworkServer object into the Handler Bus</param>
        public NetworkServer(
            string ipAddress,
            int port,
            Func<
                NetworkServer<Req, Res>,
                Func<INetworkClient<Req, Res>, Action<Transmission<Req, Res>?>>
            > transmissionHandlerServerFactory
        )
        {
            transmissionHandlerClientFactory = transmissionHandlerServerFactory(this);
            clients = new Dictionary<string, INetworkClient<Req, Res>>();
            var tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            clientConnectionListener = new ClientConnectionListener(tcpListener, RegisterNewClient);
        }

        /// <summary>
        /// Use this to get all valid clients.
        /// </summary>
        /// <returns>A list with the ids of all connected clients</returns>
        public List<string> GetListOfClientIds()
        {
            return clients.Keys.ToList();
        }

        /// <summary>
        /// Method that tries to send a transmission to a connected client
        /// </summary>
        /// <param name="targetId">Id of target client</param>
        /// <param name="transmission">Transmission to be sent</param>
        public bool TrySendTransmission(string? targetId, Transmission<Req, Res> transmission)
        {
            if (targetId != null && clients.ContainsKey(targetId))
            {
                clients[targetId].SendTransmission(transmission);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Instantiates and runs a new NetworkClient. Used as a callback for the ClientConnectionListener
        /// </summary>
        /// <param name="client">Incoming connection as a TcpClient that is created by the TcpListener</param>
        private void RegisterNewClient(TcpClient client)
        {
            var networkClient = new NetworkClient<Req, Res>(
                client,
                transmissionHandlerClientFactory,
                DisconnectClientAction
            );
            networkClient.Start();
        }

        /// <summary>
        /// Method that provides means for the Application to register a client within the Transmission handler
        /// </summary>
        /// <param name="client">NetworkClient</param>
        /// <param name="id">Id of the NetworkClient as non null</param>
        public void RegisterClientAction(INetworkClient<Req, Res> client, string id)
        {
            clients[id] = client;
            client.Id = id;
        }

        /// <summary>
        /// Provides means to manually disconnect a client
        /// </summary>
        /// <param name="id"></param>
        public void DisconnectClientAction(string? id)
        {
            if (id != null && clients.ContainsKey(id))
            {
                clients.Remove(id);
            }
        }

        /// <summary>
        /// /// Starts the TcpListener to start accepting incoming connections
        /// </summary>
        public void Start()
        {
            clientConnectionListener.StartListening();
        }

        /// <summary>
        /// Disposes the Server, safely disconnecting all clients.
        /// </summary>
        public void Dispose()
        {
            clients.ToList().ForEach(c => c.Value.Dispose());
        }
    }
}
