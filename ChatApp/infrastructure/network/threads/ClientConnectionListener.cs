using System.Net.Sockets;

namespace Network
{
    /// <summary>
    /// Runs listener for incoming client connections in separate thread.
    /// Executes action on connection
    /// </summary>
    public class ClientConnectionListener
    {
        private TcpListener listener;
        private Thread thread;

        /// <summary>
        /// /// Callback provided by the NetworkServer to register new Clients
        /// </summary>
        private Action<TcpClient> action;

        /// <summary>
        /// Used to initiate delegate for the separate Thread,
        /// </summary>
        private void Listen()
        {
            TcpClient client;

            listener.Start();
            // TODO: Add a lock
            try
            {
                while (listenerLock)
                {
                    client = listener.AcceptTcpClient();
                    action(client);
                }
            }
            catch (SocketException) { }
            // listener.Stop();
        }

        private bool listenerLock;

        /// <summary>
        /// Savely stop the tcplistener
        /// </summary>
        public void Stop()
        {
            listenerLock = false;
            listener.Stop();
        }

        /// <summary>
        /// Constructor to be called by NetworkServer
        /// </summary>
        /// <param name="tcpListener">tspListener with running socket provided by NetworkServer</param>
        /// <param name="registerAction">Callback provided by NetworkServer to register new clients</param>
        public ClientConnectionListener(TcpListener tcpListener, Action<TcpClient> registerAction)
        {
            listener = tcpListener;
            listenerLock = false;
            action = registerAction;
            thread = new Thread(new ThreadStart(Listen));
        }

        /// <summary>
        /// Starts the thread thus executing the Listen method in a new thread.
        /// </summary>
        public void StartListening()
        {
            listenerLock = true;
            thread.Start();
        }
    }
}
