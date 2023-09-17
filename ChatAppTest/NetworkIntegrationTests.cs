namespace Network;

[TestClass]
public class NetworkIntegrationTests
{
    private readonly string id = "test";
    private readonly string registerTransmission = "register";
    private readonly string disconnectTransmission = "disconnect";
    private readonly string ip = "127.0.0.1";

    private class TestHandlers
    {
        public INetworkServer<string, string>? serverHandlerServer;
        public INetworkClient<string, string>? serverHandlerClient;
        public Transmission<string, string>? serverHandlerTransmission;

        public Func<
            INetworkClient<string, string>,
            Action<Transmission<string, string>?>
        > ServerHandler(INetworkServer<string, string> server)
        {
            serverHandlerServer = server;
            return (c) =>
            {
                serverHandlerClient = c;
                return (t) =>
                {
                    serverHandlerTransmission = t;
                };
            };
        }

        public INetworkClient<string, string>? clientHandlerClient;
        public Transmission<string, string>? clientHandlerTransmission;

        public Action<Transmission<string, string>?> ClientHandler(
            INetworkClient<string, string> client
        )
        {
            clientHandlerClient = client;
            return (t) =>
            {
                clientHandlerTransmission = t;
            };
        }
    }

    [TestMethod]
    public void ServerReceivesRegisterRequest()
    {
        int port = 1234;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port, (s) => handlers.ServerHandler(s));
        server.Start();
        Thread.Sleep(200);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            (c) => handlers.ClientHandler(c),
            registerTransmission,
            disconnectTransmission
        );
        client.Start();
        Thread.Sleep(200);

        Assert.AreEqual(server, handlers.serverHandlerServer);
        Assert.IsNotNull(handlers.serverHandlerClient);
        Assert.AreEqual(registerTransmission, handlers.serverHandlerTransmission?.request);
    }

    [TestMethod]
    public void ServerCanRegisterClient()
    {
        int port = 1235;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port, (s) => handlers.ServerHandler(s));
        server.Start();
        Thread.Sleep(200);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            (c) => handlers.ClientHandler(c),
            registerTransmission,
            disconnectTransmission
        );
        client.Start();
        Thread.Sleep(200);
        Assert.IsNotNull(handlers.serverHandlerClient);
        server.RegisterClientAction(handlers.serverHandlerClient, id);

        Assert.IsTrue(server.clients.ContainsKey(id));
    }

    [TestMethod]
    public void ClientCanSendMessage()
    {
        int port = 1236;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port, (s) => handlers.ServerHandler(s));
        server.Start();
        Thread.Sleep(200);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            (c) => handlers.ClientHandler(c),
            registerTransmission,
            disconnectTransmission
        );
        client.Start();
        Thread.Sleep(200);
        Assert.IsNotNull(handlers.serverHandlerClient);
        server.RegisterClientAction(handlers.serverHandlerClient, id);

        string message = "message";
        client.SendClientRequest(id, message);
        Thread.Sleep(200);

        Assert.AreEqual(server.clients[id], handlers.serverHandlerClient);
        Assert.AreEqual(message, handlers.serverHandlerTransmission?.request);
    }

    [TestMethod]
    public void ClientCanReceiveMessage()
    {
        int port = 1237;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port, (s) => handlers.ServerHandler(s));
        server.Start();
        Thread.Sleep(200);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            (c) => handlers.ClientHandler(c),
            registerTransmission,
            disconnectTransmission
        );
        client.Start();
        Thread.Sleep(200);
        Assert.IsNotNull(handlers.serverHandlerClient);
        server.RegisterClientAction(handlers.serverHandlerClient, id);

        Assert.IsTrue(server.clients.ContainsKey(id));
        string message = "message";
        server.clients[id].SendClientRequest(id, message);
        Thread.Sleep(200);

        Assert.AreEqual(client, handlers.clientHandlerClient);
        Assert.AreEqual(message, handlers.clientHandlerTransmission?.request);
    }

    [TestMethod]
    public void ServerReceivesDisconnectRequest()
    {
        int port = 1238;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port, (s) => handlers.ServerHandler(s));
        server.Start();
        Thread.Sleep(200);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            (c) => handlers.ClientHandler(c),
            registerTransmission,
            disconnectTransmission
        );
        client.Start();
        Thread.Sleep(200);
        Assert.IsNotNull(handlers.serverHandlerClient);
        server.RegisterClientAction(handlers.serverHandlerClient, id);

        Assert.IsTrue(server.clients.ContainsKey(id));
        client.Dispose();
        Thread.Sleep(200);

        Assert.AreEqual(server, handlers.serverHandlerServer);
        Assert.IsNotNull(handlers.serverHandlerClient);
        Assert.AreEqual(disconnectTransmission, handlers.serverHandlerTransmission?.request);
    }
}
