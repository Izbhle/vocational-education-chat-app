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
        private readonly Action<ITransmission<Req, Res>?> transmissionHandler;
        private readonly Thread thread;

        /// <summary>
        /// Used as a delegate to run the thread. Listens for data, converts it into a Transmission and executes the TransmissionHandler with that.
        /// </summary>
        private void ReadStream()
        {
            byte[] bytes = new byte[4096];
            string? dataAsString;
            TransmissionWrapper<Req, Res> transmission;

            int numberOfBytes;
            while (streamLock)
            {
                numberOfBytes = stream.Read(bytes, 0, bytes.Length);
                dataAsString = Encoding.UTF8.GetString(bytes, 0, numberOfBytes);
                transmission = new TransmissionWrapper<Req, Res>(dataAsString);
                transmissionHandler(transmission.data);
            }
        }

        /// <summary>
        /// Constructor to be used by NetworkClient
        /// </summary>
        /// <param name="tcpStream">Uses an already established stream</param>
        /// <param name="transmissionAction">Transmission Handler to be executed for each Transmission</param>
        /// <param name="closeAction">Action to execute when the stream closes</param>
        public TransmissionReceiver(
            NetworkStream tcpStream,
            Action<ITransmission<Req, Res>?> transmissionAction
        )
        {
            stream = tcpStream;
            transmissionHandler = transmissionAction;
            thread = new Thread(new ThreadStart(ReadStream));
        }

        /// <summary>
        /// Starts the new Thread with executing ReadStream
        /// </summary>
        public void StartListening()
        {
            streamLock = true;
            thread.Start();
        }

        private bool streamLock;

        /// <summary>
        /// Savely stop the stream
        /// </summary>
        public void Stop()
        {
            streamLock = false;
            stream.Close();
        }
    }
}
