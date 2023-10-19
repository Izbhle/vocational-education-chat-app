using System.Net.Sockets;
using System.Text;
using Transmissions;

namespace Network
{
    /// <summary>
    /// Used by both the Server and Client applications to communicate with Transmissions.
    /// </summary>
    /// <typeparam name="Req">Request</typeparam>
    /// <typeparam name="Res">Response</typeparam>
    public class NetworkClient<Req, Res> : INetworkClient<Req, Res>
    {
        private string? _Id;

        /// <summary>
        /// Argument for Id can only be assigned once
        /// </summary>
        public string? Id
        {
            get { return _Id; }
            set { _Id ??= value; }
        }

        /// <summary>
        /// Serves as the network connection
        /// </summary>
        private TcpClient? tcpClient;

        /// <summary>
        /// All communication is done via this single stream
        /// </summary>
        private NetworkStream? stream;

        private TransmissionReceiver<Req, Res>? transmissionReceiver;
        private Action<ITransmission<Req, Res>?>? transmissionHandler;

        /// <summary>
        /// This is the initial request to be send to the server to register this client, used by client applications
        /// </summary>
        private readonly Req? registerRequest;

        /// <summary>
        /// This request will be sent uppon disconnect
        /// </summary>
        private readonly Req? disconnectRequest;

        /// <summary>
        /// Callback to execute on the server when a stream closes
        /// </summary>
        private readonly Action<string?>? closeStreamServerAction;
        private readonly string? ip;
        private readonly int? port;

        /// <summary>
        /// Constructor to be used by the Client Application
        /// </summary>
        /// <param name="initialId">id this client should use</param>
        /// <param name="ipAddress">Server id</param>
        /// <param name="networkPort">Server port</param>
        /// <param name="transmissionHandlerFactory">Used to inject NetworkClient object into the transmission handler</param>
        /// <param name="serverRegisterRequest"></param>
        public NetworkClient(
            string initialId,
            string ipAddress,
            int networkPort,
            Req serverRegisterRequest,
            Req serverDisconnectRequest
        )
        {
            Id = initialId;
            ip = ipAddress;
            port = networkPort;
            tcpClient = new TcpClient(ip, (int)port); // Start the network connection to the server
            registerRequest = serverRegisterRequest;
            disconnectRequest = serverDisconnectRequest;
        }

        /// <summary>
        /// Constructor to be used by the NetworkServer
        /// </summary>
        /// <param name="client">tcpCLient that is returned by the tcpListener</param>
        /// <param name="transmissionHandlerFactory">Used to inject NetworkClient object into the transmission handler</param>
        /// <param name="streamCloseAction">Execute this action on the server when the stream closes</param>
        public NetworkClient(TcpClient client, Action<string?> streamCloseAction)
        {
            tcpClient = client;
            closeStreamServerAction = streamCloseAction;
        }

        /// <summary>
        /// Try to get a running stream. For clients of the server this always returns false.
        /// </summary>
        /// <returns><c>true</c> on success, <c>false</c> otherwise</returns>
        private bool TryStartStream()
        {
            try
            {
                if (tcpClient != null)
                {
                    if (!tcpClient.Connected) // Reinitialize client on disconnect
                    {
                        if (port == null || ip == null)
                        {
                            return false;
                        }
                        tcpClient = new TcpClient(ip, (int)port);
                    }
                }
                else
                {
                    if (port == null || ip == null)
                    {
                        return false;
                    }
                    tcpClient = new TcpClient(ip, (int)port);
                }
                if (stream == null) // Reinitialize stream on disconnect, also needs new receiver thread as the old one ends.
                {
                    stream = tcpClient.GetStream();
                    transmissionReceiver = new TransmissionReceiver<Req, Res>(
                        stream,
                        transmissionHandler!
                    );
                    transmissionReceiver.StartListening();
                }
                if (registerRequest != null) // New connection needs to be registered on the server
                {
                    SendServerRequest(registerRequest);
                }
                return true;
            }
            catch (SocketException) // When server is not reachable
            {
                return false;
            }
        }

        /// <summary>
        /// Starts the stream and the receiver thread.
        /// </summary>
        /// <param name="transmissionHandlerFactory">Transmission Handler</param>
        public void Start(
            Func<
                NetworkClient<Req, Res>,
                Action<ITransmission<Req, Res>?>
            > transmissionHandlerFactory
        )
        {
            transmissionHandler = transmissionHandlerFactory(this);
            TryStartStream();
        }

        /// <summary>
        /// Safely closes all connections. This also puts tcpClient into an unusuable state, only use this when you mean to dispose the client
        /// </summary>
        public void Dispose()
        {
            closeStreamServerAction?.Invoke(Id); // Also execute server callback when it exists.
            if (disconnectRequest != null)
                SendServerRequest(disconnectRequest);
            Thread.Sleep(200);
            transmissionReceiver?.Stop();
            tcpClient?.Close();
            return;
        }

        /// <summary>
        /// Sends a transmission to the server
        /// </summary>
        /// <param name="transmission for the server">Transmission</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise</returns>

        public bool TrySendTransmission(ITransmission<Req, Res> transmission)
        {
            var transmissionWrapper = new TransmissionWrapper<Req, Res>(transmission);
            return TrySendData(transmissionWrapper.jsonString);
        }

        /// <summary>
        /// Send a client request to the server
        /// </summary>
        /// <param name="receiverId">ID of the target client</param>
        /// <param name="request">Request</param>
        /// <returns><c>Transmission</c> if success, <c>null</c> otherwise</returns>
        public ITransmission<Req, Res>? SendClientRequest(string receiverId, Req request)
        {
            if (Id == null)
            {
                return null;
            }
            var transmission = new Transmission<Req, Res>
            {
                transmissionType = TransmissionType.request,
                targetType = TargetType.client,
                receiverId = receiverId,
                senderId = Id,
                request = request
            };
            if (TrySendTransmission(transmission))
            {
                return transmission;
            }
            return null;
        }

        /// <summary>
        /// Send a server request to the server
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns><c>Transmission</c> if success, <c>null</c> otherwise</returns>
        public ITransmission<Req, Res>? SendServerRequest(Req request)
        {
            if (Id == null)
            {
                return null;
            }
            var transmission = new Transmission<Req, Res>
            {
                transmissionType = TransmissionType.request,
                targetType = TargetType.server,
                senderId = Id,
                request = request
            };
            if (TrySendTransmission(transmission))
            {
                return transmission;
            }
            return null;
        }

        /// <summary>
        /// Sends a response for a request transmission
        /// </summary>
        /// <param name="oldTransmission">Request transmission</param>
        /// <param name="response">Response to be send</param>
        /// <returns>send <c>Transmission</c> if success, <c>null</c> otherwise</returns>
        public ITransmission<Req, Res>? SendResponse(
            ITransmission<Req, Res> oldTransmission,
            Res response
        )
        {
            var transmission = new Transmission<Req, Res>
            {
                transmissionType = TransmissionType.response,
                targetType = oldTransmission.targetType,
                senderId = oldTransmission.receiverId,
                receiverId = oldTransmission.senderId,
                response = response
            };
            if (TrySendTransmission(transmission))
            {
                return transmission;
            }
            return null;
        }

        /// <summary>
        /// Tries to send data string to the server via stream. Tries to reconnect automatically when the stream is down.
        /// </summary>
        /// <param name="data">presumably json String</param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise</returns>

        private bool TrySendData(string data)
        {
            if (stream == null) // Ensure the stream is running
            {
                if (!TryStartStream())
                {
                    return false;
                }
            }
            byte[] rawData = Encoding.UTF8.GetBytes(data); // data send as bytes
            stream?.Write(rawData, 0, rawData.Length);
            // Console.WriteLine("Sent    : " + data);
            return true;
        }
    }
}
