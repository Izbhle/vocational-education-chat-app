using Transmissions;

using ChatAppClient;
using ChatAppServer;

[TestClass]
public class ChatIntegrationTests
{
    private readonly string ip = "127.0.0.1";

    [TestMethod]
    public void ChatClientsCanCommunicate()
    {
        string message = "message";
        int port = 1247;
        var server = ChatServer.CreateNew(ip, port);
        server.Start();
        Thread.Sleep(100);
        var clientA = ChatClient.CreateNew(
            "A",
            ip,
            port,
            () => { },
            (ChatLogType type, string message) => { }
        );
        clientA.Start();
        Thread.Sleep(100);
        var clientB = ChatClient.CreateNew(
            "B",
            ip,
            port,
            () => { },
            (ChatLogType type, string message) => { }
        );
        clientB.Start();
        Thread.Sleep(100);
        clientA.SendMessage("B", message);
        Thread.Sleep(100);
        // Check that ClientB receives and stores the message
        Assert.IsTrue(
            clientB.MessagesStore.RequestTransmissions["A"].Values.First()!.senderId!.Equals("A")
        );
        Assert.IsTrue(
            clientB.MessagesStore.RequestTransmissions["A"].Values.First()!.receiverId!.Equals("B")
        );
        Assert.IsTrue(
            clientB.MessagesStore.RequestTransmissions["A"].Values
                .First()!
                .request!.message!.Equals(message)
        );
        // Check that ClientA saves the send message and stores the response
        Assert.IsTrue(
            clientA.MessagesStore.RequestTransmissions["B"].Values.First()!.senderId!.Equals("A")
        );
        Assert.IsTrue(
            clientA.MessagesStore.RequestTransmissions["B"].Values.First()!.receiverId!.Equals("B")
        );
        Assert.IsTrue(
            clientA.MessagesStore.RequestTransmissions["B"].Values
                .First()!
                .request!.message!.Equals(message)
        );
        Assert.IsTrue(
            clientA.MessagesStore.RequestTransmissions["B"].Values
                .First()!
                .response!.requestType!.Equals(ChatRequestType.Message)
        );
        clientA.Dispose();
        clientB.Dispose();
        server.Dispose();
    }
}
