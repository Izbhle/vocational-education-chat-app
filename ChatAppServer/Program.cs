namespace ChatAppServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Server...");
            string ip = "";
            int port = 0;
            foreach (string arg in args)
            {
                if (arg.StartsWith("--ip="))
                {
                    string[] parts = arg.Split("=");
                    if (parts.Length != 2)
                    {
                        throw new Exception("Illegal Arguments");
                    }
                    ip = parts[1];
                }
                else if (arg.StartsWith("--port="))
                {
                    string[] parts = arg.Split("=");
                    if (parts.Length != 2)
                    {
                        throw new Exception("Illegal Arguments");
                    }
                    int.TryParse(parts[1], out port);
                }
            }
            if (string.IsNullOrEmpty(ip) || port == 0)
            {
                Console.WriteLine(port);
                Console.WriteLine(ip);
                throw new Exception("Illegal Arguments");
            }
            ChatServer server = ChatServer.CreateNew(ip, port);
            server.Start();
            Console.WriteLine("...success");
            Console.WriteLine($"Server is now running and listening on {ip}:{port}");
        }
    }
}
