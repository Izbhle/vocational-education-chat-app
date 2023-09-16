using ReactiveUI;

namespace Network;

[TestClass]
public class NetworkIntegrationTests
{
    private readonly string id = "test";
    private readonly string registerTransmission = "register";
    private readonly string disconnectTransmission = "disconnect";
    private readonly string ip = "127.0.0.1";

    class TestHandlers
    {
        public NetworkServer<string, string>? serverHandlerServer;
        public NetworkClient<string, string>? serverHandlerClient;
        public Transmission<string, string>? serverHandlerTransmission;

        public Func<
            NetworkClient<string, string>,
            Action<Transmission<string, string>?>
        > ServerHandler(NetworkServer<string, string> server)
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

        public NetworkClient<string, string>? clientHandlerClient;
        public Transmission<string, string>? clientHandlerTransmission;

        public Action<Transmission<string, string>?> ClientHandler(
            NetworkClient<string, string> client
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
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, 1234, (s) => handlers.ServerHandler(s));
        server.Start();
        Thread.Sleep(200);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            1234,
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
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, 1235, (s) => handlers.ServerHandler(s));
        server.Start();
        Thread.Sleep(200);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            1235,
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
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, 1236, (s) => handlers.ServerHandler(s));
        server.Start();
        Thread.Sleep(200);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            1236,
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
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, 1237, (s) => handlers.ServerHandler(s));
        server.Start();
        Thread.Sleep(200);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            1237,
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
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, 1237, (s) => handlers.ServerHandler(s));
        server.Start();
        Thread.Sleep(200);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            1237,
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
