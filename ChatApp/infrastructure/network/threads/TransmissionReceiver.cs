using System.Net.Sockets;
using System.Text;

namespace Network
{
    /// <summary>
    /// Launches a listener in a separate thread that executes the Transmission Handler for all incomming Transmissions.
    /// </summary>
    /// <typeparam name="Req">Request</typeparam>
    /// <typeparam name="Res">Response</typeparam>
    public class TransmissionReceiver<Req, Res>
    {
        private readonly NetworkStream stream;
        private readonly Action<Transmission<Req, Res>?> transmissionHandler;

        /// <summary>
        /// Action to execute when the stream closes
        /// </summary>
        private readonly Action? closeStreamAction;
        private readonly Thread thread;

        /// <summary>
        /// Used as a delegate to run the thread. Listens for data, converts it into a Transmission and executes the TransmissionHandler with that.
        /// </summary>
        private void ReadStream()
        {
            byte[] bytes = new byte[256];
            string? data;
            TransmissionWrapper<Req, Res> transmission;

            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) // Read incoming data, when the data is empty or the commection is closed the loop exits
                {
                    data = Encoding.UTF8.GetString(bytes, 0, i); // Bytes needs to be converted into UTF8 string
                    transmission = new TransmissionWrapper<Req, Res>(data);
                    transmissionHandler(transmission.data);
                }
            }
            catch (IOException) { }
            closeStreamAction?.Invoke();
        }

        /// <summary>
        /// Constructor to be used by NetworkClient
        /// </summary>
        /// <param name="tcpStream">Uses an already established stream</param>
        /// <param name="transmissionAction">Transmission Handler to be executed for each Transmission</param>
        /// <param name="closeAction">Action to execute when the stream closes</param>
        public TransmissionReceiver(
            NetworkStream tcpStream,
            Action<Transmission<Req, Res>?> transmissionAction,
            Action? closeAction
        )
        {
            stream = tcpStream;
            transmissionHandler = transmissionAction;
            closeStreamAction = closeAction;
            thread = new Thread(new ThreadStart(ReadStream));
        }

        /// <summary>
        /// Starts the new Thread with executing ReadStream
        /// </summary>
        public void StartListening()
        {
            thread.Start();
        }
    }
}
