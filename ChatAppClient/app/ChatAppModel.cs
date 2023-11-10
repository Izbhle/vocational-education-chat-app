using ChatAppServer;

namespace ChatAppClient
{
    public class ChatAppModel
    {
        public IChatClient? Client { get; set; }
        public IChatServer? Server { get; set; }
        public string Name { get; set; } = "Chose your Name";
        public delegate void Callback();

        public Callback? OnTransmissionCallback { get; set; }

        public Callback? OnStartupCallback { get; set; }

        public Callback? OnExitCallback { get; set; }

        public void Dispose()
        {
            Client?.Dispose();
            Thread.Sleep(200);
            Server?.Dispose();
            Thread.Sleep(200);
        }
    }
}
