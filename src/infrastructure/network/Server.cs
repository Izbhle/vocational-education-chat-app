using System.Net;
using System.Net.Sockets;

namespace Network
{
    /// <summary>
    /// Allows incomming clients to connect. Provies network communcation via Transmissions.
    /// </summary>
    /// <typeparam name="Req">Request</typeparam>
    /// <typeparam name="Res">Response</typeparam>
    class NetworkServer<Req, Res>
    {
        /// <summary>
        /// Reference all clients by clientId. Used to Relay Transmissions.
        /// </summary>
        public readonly Dictionary<string, NetworkClient<Req, Res>> clients;
        private readonly ClientConnectionListener clientConnectionListener;

        /// <summary>
        /// Factory function used to inject the NetworkClient object into the Handler Bus
        /// </summary>
        /// <returns>Transmission handler provided by the Application</returns>
        private readonly Func<
            NetworkClient<Req, Res>,
            Action<Transmission<Req, Res>?>
        > transmissionHandlerClientFactory;

        /// <summary>
        ///
        /// </summary>
        /// <param name="ipAdress">IP Adress of the NetworkServer</param>
        /// <param name="port">Port of the NetworkServer</param>
        /// <param name="transmissionHandlerServerFactory">Factory function used to inject the NetworkServer object into the Handler Bus</param>
        public NetworkServer(
            string ipAdress,
            int port,
            Func<
                NetworkServer<Req, Res>,
                Func<NetworkClient<Req, Res>, Action<Transmission<Req, Res>?>>
            > transmissionHandlerServerFactory
        )
        {
            transmissionHandlerClientFactory = transmissionHandlerServerFactory(this);
            clients = new Dictionary<string, NetworkClient<Req, Res>>();
            var tcpListener = new TcpListener(IPAddress.Parse(ipAdress), port);
            clientConnectionListener = new ClientConnectionListener(tcpListener, RegisterNewClient);
        }

        /// <summary>
        /// Instantiates and runs a new Networkclient. Used as a callback for the ClientConnectionListener
        /// </summary>
        /// <param name="client">Incomming connection as a TcpClient that is created by the TcpListener</param>
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
        public void RegisterClientAction(NetworkClient<Req, Res> client, string id)
        {
            clients[id] = client;
        }

        /// <summary>
        /// Provides means to manually disconnect a client
        /// </summary>
        /// <param name="id"></param>
        public void DisconnectClientAction(string id)
        {
            clients.Remove(id);
        }

        /// <summary>
        /// /// Starts the TcpListener to start accepting incomming connections
        /// </summary>
        public void Start()
        {
            clientConnectionListener.StartListening();
        }
    }
}
