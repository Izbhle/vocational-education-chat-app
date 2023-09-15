using System.Net.Sockets;
using System.Text;

namespace Network
{
    /// <summary>
    /// Used by both the Server and Client applications to communicate with Transmissions.
    /// </summary>
    /// <typeparam name="Req">Request</typeparam>
    /// <typeparam name="Res">Response</typeparam>
    public class NetworkClient<Req, Res>
    {
        protected string? id;

        /// <summary>
        /// Serves as the network connection
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// All communication is done via this single stream
        /// </summary>
        private NetworkStream? stream;

        private TransmissionReceiver<Req, Res>? transmissionReceiver;
        private readonly Action<Transmission<Req, Res>?> transmissionHandler;

        /// <summary>
        /// This is the initial request to be send to the server to register this client, used by client applications
        /// </summary>
        private readonly Req? registerRequest;

        /// <summary>
        /// Callback to execute on the server when a stream closes
        /// </summary>
        private readonly Action<string>? closeStreamServerAction;
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
            Func<
                NetworkClient<Req, Res>,
                Action<Transmission<Req, Res>?>
            > transmissionHandlerFactory,
            Req serverRegisterRequest
        )
        {
            id = initialId;
            ip = ipAddress;
            port = networkPort;
            tcpClient = new TcpClient(ip, (int)port); // Start the network connection to the server
            registerRequest = serverRegisterRequest;
            transmissionHandler = transmissionHandlerFactory(this);
        }

        /// <summary>
        /// Constructor to be used by the NetworkServer
        /// </summary>
        /// <param name="client">tcpCLient that is returned by the tcpListener</param>
        /// <param name="transmissionHandlerFactory">Used to inject NetworkClient object into the transmission handler</param>
        /// <param name="streamCloseAction">Execute this action on the server when the stream closes</param>
        public NetworkClient(
            TcpClient client,
            Func<
                NetworkClient<Req, Res>,
                Action<Transmission<Req, Res>?>
            > transmissionHandlerFactory,
            Action<string> streamCloseAction
        )
        {
            tcpClient = client;
            transmissionHandler = transmissionHandlerFactory(this);
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
                if (!tcpClient.Connected) // Reinitialize client on disconnect
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
                        transmissionHandler,
                        CloseStreamAction
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
        /// Callback to execute on the client when the stream closes in the receiver thread.
        /// This safely shuts down the stream, in order to cleanly restart it later.
        /// </summary>
        private void CloseStreamAction()
        {
            transmissionReceiver = null;
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
            if (id != null)
            {
                closeStreamServerAction?.Invoke(id); // Also execute server callback when it exists.
            }
        }

        /// <summary>
        /// Starts the stream and the receiver thread.
        /// </summary>
        public void Start()
        {
            TryStartStream();
        }

        /// <summary>
        /// Stops the stream and the receiver thread.
        /// </summary>
        public void Stop()
        {
            if (stream == null)
            {
                return;
            }
            stream.Dispose();
            stream = null;
        }

        /// <summary>
        /// Sends a transmission to the server
        /// </summary>
        /// <param name="transmission for the server">Transmission</param>
        public void SendTransmission(Transmission<Req, Res> transmission)
        {
            var transmissionWrapper = new TransmissionWrapper<Req, Res>(transmission);
            TrySendData(transmissionWrapper.jsonString);
        }

        /// <summary>
        /// Send a client request to the server
        /// </summary>
        /// <param name="receiverId">ID of the target client</param>
        /// <param name="request">Request</param>
        /// <returns><c>Transmission</c> if success, <c>null</c> otherwise</returns>
        public Transmission<Req, Res>? SendClientRequest(string receiverId, Req request)
        {
            if (id == null)
            {
                return null;
            }
            var transmission = new TransmissionWrapper<Req, Res>(
                new Transmission<Req, Res>
                {
                    transmissionType = TransmissionType.request,
                    targetType = TargetType.client,
                    receiverId = receiverId,
                    senderId = id,
                    request = request
                }
            );
            if (TrySendData(transmission.jsonString))
            {
                return transmission.data;
            }
            return null;
        }

        /// <summary>
        /// Send a server request to the server
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns><c>Transmission</c> if success, <c>null</c> otherwise</returns>
        public Transmission<Req, Res>? SendServerRequest(Req request)
        {
            if (id == null)
            {
                return null;
            }
            var transmission = new TransmissionWrapper<Req, Res>(
                new Transmission<Req, Res>
                {
                    transmissionType = TransmissionType.request,
                    targetType = TargetType.server,
                    senderId = id,
                    request = request
                }
            );
            if (TrySendData(transmission.jsonString))
            {
                return transmission.data;
            }
            return null;
        }

        /// <summary>
        /// Sends a response for a request transmission
        /// </summary>
        /// <param name="oldTransmission">Request transmission</param>
        /// <param name="response">Response to be send</param>
        /// <returns>send <c>Transmission</c> if success, <c>null</c> otherwise</returns>
        public Transmission<Req, Res>? SendResponse(
            Transmission<Req, Res> oldTransmission,
            Res response
        )
        {
            if (id == null)
            {
                return null;
            }
            var transmission = new TransmissionWrapper<Req, Res>(
                new Transmission<Req, Res>
                {
                    transmissionType = TransmissionType.response,
                    targetType = oldTransmission.targetType,
                    senderId = oldTransmission.senderId,
                    receiverId = id,
                    response = response
                }
            );
            if (TrySendData(transmission.jsonString))
            {
                return transmission.data;
            }
            return null;
        }
        /// <summary>
        /// Allows initializing the protected Id field if it is null. Does nothing if the id is already set.
        /// </summary>
        /// <param name="newId">Id to initializes</param>
        public void InitializeId(string newId)
        {
            id ??= newId;
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
            stream!.Write(rawData, 0, rawData.Length);
            // Console.WriteLine("Sent    : " + data);
            return true;
        }
    }
}
