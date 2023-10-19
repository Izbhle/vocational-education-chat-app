using Transmissions;

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
        public ITransmission<string, string>? serverHandlerTransmission;

        public Func<
            INetworkClient<string, string>,
            Action<ITransmission<string, string>?>
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
        public ITransmission<string, string>? clientHandlerTransmission;

        public Action<ITransmission<string, string>?> ClientHandler(
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
        int port = 1240;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port);
        server.Start(handlers.ServerHandler);
        Thread.Sleep(20);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            registerTransmission,
            disconnectTransmission
        );
        client.Start(handlers.ClientHandler);
        Thread.Sleep(20);

        Assert.AreEqual(server, handlers.serverHandlerServer);
        Assert.IsNotNull(handlers.serverHandlerClient);
        Assert.AreEqual(registerTransmission, handlers.serverHandlerTransmission?.request);
        client.Dispose();
        server.Dispose();
    }

    [TestMethod]
    public void ServerCanRegisterClient()
    {
        int port = 1241;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port);
        server.Start(handlers.ServerHandler);
        Thread.Sleep(20);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            registerTransmission,
            disconnectTransmission
        );
        client.Start(handlers.ClientHandler);
        Thread.Sleep(20);
        Assert.IsNotNull(handlers.serverHandlerClient);
        server.RegisterClientAction(handlers.serverHandlerClient, id);
        Assert.IsTrue(server.GetListOfClientIds().Contains(id));
        client.Dispose();
        server.Dispose();
    }

    [TestMethod]
    public void ClientCanSendMessage()
    {
        int port = 1242;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port);
        server.Start(handlers.ServerHandler);
        Thread.Sleep(20);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            registerTransmission,
            disconnectTransmission
        );
        client.Start(handlers.ClientHandler);
        Thread.Sleep(20);
        Assert.IsNotNull(handlers.serverHandlerClient);
        server.RegisterClientAction(handlers.serverHandlerClient, id);

        string message = "message";
        client.SendClientRequest(id, message);
        Thread.Sleep(20);
        Assert.AreEqual(server.GetClient(id), handlers.serverHandlerClient);
        Assert.AreEqual(message, handlers.serverHandlerTransmission?.request);
        client.Dispose();
        server.Dispose();
    }

    [TestMethod]
    public void ClientCanReceiveMessage()
    {
        int port = 1243;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port);
        server.Start(handlers.ServerHandler);
        Thread.Sleep(20);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            registerTransmission,
            disconnectTransmission
        );
        client.Start(handlers.ClientHandler);
        Thread.Sleep(20);
        Assert.IsNotNull(handlers.serverHandlerClient);
        server.RegisterClientAction(handlers.serverHandlerClient, id);

        Assert.IsTrue(server.GetListOfClientIds().Contains(id));
        string message = "message";
        server.GetClient(id)?.SendClientRequest(id, message);
        Thread.Sleep(20);

        Assert.AreEqual(client, handlers.clientHandlerClient);
        Assert.AreEqual(message, handlers.clientHandlerTransmission?.request);
        client.Dispose();
        server.Dispose();
    }

    [TestMethod]
    public void ServerReceivesDisconnectRequest()
    {
        int port = 1244;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port);
        server.Start(handlers.ServerHandler);
        Thread.Sleep(20);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            registerTransmission,
            disconnectTransmission
        );
        client.Start(handlers.ClientHandler);
        Thread.Sleep(20);
        Assert.IsNotNull(handlers.serverHandlerClient);
        server.RegisterClientAction(handlers.serverHandlerClient, id);
        Assert.IsTrue(server.GetListOfClientIds().Contains(id));
        client.Dispose();
        Thread.Sleep(20);

        Assert.AreEqual(server, handlers.serverHandlerServer);
        Assert.IsNotNull(handlers.serverHandlerClient);
        Assert.AreEqual(disconnectTransmission, handlers.serverHandlerTransmission?.request);
        server.Dispose();
    }

    [TestMethod]
    public void ClientCanSendResponse()
    {
        int port = 1245;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port);
        server.Start(handlers.ServerHandler);
        Thread.Sleep(20);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            registerTransmission,
            disconnectTransmission
        );
        client.Start(handlers.ClientHandler);
        Thread.Sleep(20);
        Assert.IsNotNull(handlers.serverHandlerClient);
        server.RegisterClientAction(handlers.serverHandlerClient, id);

        var requestTransmission = new Transmission<string, string>
        {
            targetType = TargetType.client,
            transmissionType = TransmissionType.request,
            senderId = "other",
            receiverId = id,
            request = "request"
        };

        Assert.IsTrue(server.GetListOfClientIds().Contains(id));

        string response = "response";
        client.SendResponse(requestTransmission, response);
        Thread.Sleep(20);

        Assert.AreEqual(server, handlers.serverHandlerServer);
        Assert.IsNotNull(handlers.serverHandlerClient);
        Assert.AreEqual(response, handlers.serverHandlerTransmission?.response);
        client.Dispose();
        server.Dispose();
    }

    [TestMethod]
    public void ServerSendsMessagesToAll()
    {
        int port = 1246;
        var handlers = new TestHandlers();
        var server = new NetworkServer<string, string>(ip, port);
        server.Start(handlers.ServerHandler);
        Thread.Sleep(20);
        var client = new NetworkClient<string, string>(
            id,
            ip,
            port,
            registerTransmission,
            disconnectTransmission
        );
        client.Start(handlers.ClientHandler);
        Thread.Sleep(20);
        Assert.IsNotNull(handlers.serverHandlerClient);
        server.RegisterClientAction(handlers.serverHandlerClient, id);

        string response = "response";
        Assert.IsTrue(server.GetListOfClientIds().Contains(id));

        server.SendResponseToAllClients(response);
        Thread.Sleep(20);

        Assert.AreEqual(client, handlers.clientHandlerClient);
        Assert.AreEqual(response, handlers.clientHandlerTransmission?.response);
        client.Dispose();
        server.Dispose();
    }
}
