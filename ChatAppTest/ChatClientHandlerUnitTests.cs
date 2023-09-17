using Network;

namespace ChatApp;

[TestClass]
public class ChatClientHandlerUnitTests
{
    private readonly Mock<INetworkClient<ChatRequest, ChatResponse>> clientMock = new();
    private readonly Mock<IChatClient> chatClientMock = new();
    private readonly Mock<IChatRequestStore> receivedMessagesStoreMock = new();
    private readonly Mock<IChatRequestStore> sendMessagesStoreMock = new();
    private readonly Mock<Action> callbackMock = new();

    public ChatClientHandlerUnitTests()
    {
        chatClientMock
            .SetupGet(c => c.receivedMessagesStore)
            .Returns(receivedMessagesStoreMock.Object);
        chatClientMock.SetupGet(c => c.sendMessagesStore).Returns(sendMessagesStoreMock.Object);
        chatClientMock.SetupGet(c => c.callback).Returns(callbackMock.Object);
        chatClientMock.SetupProperty(c => c.availableClients);
    }

    [TestMethod]
    public void StoresValidReceivedMessage()
    {
        string message = "message";
        string senderName = "sender";
        string receiverName = "receiver";
        var transmission = new Transmission<ChatRequest, ChatResponse>
        {
            transmissionType = TransmissionType.request,
            targetType = TargetType.client,
            senderId = senderName,
            receiverId = receiverName,
            request = new ChatRequest
            {
                requestType = ChatRequestType.Message,
                message = message,
                requestTimeId = DateTime.UtcNow
            }
        };

        ChatClientHandler.TransmissionHandler(
            chatClientMock.Object,
            clientMock.Object,
            transmission
        );
        receivedMessagesStoreMock.Verify(s => s.Store(transmission), Times.Exactly(1));
        sendMessagesStoreMock.Verify(
            s => s.Store(It.IsAny<Transmission<ChatRequest, ChatResponse>>()),
            Times.Never()
        );
        callbackMock.Verify(c => c());
    }

    [TestMethod]
    public void StoresValidResponseMessage()
    {
        string senderName = "sender";
        string receiverName = "receiver";
        var transmission = new Transmission<ChatRequest, ChatResponse>
        {
            transmissionType = TransmissionType.response,
            targetType = TargetType.client,
            senderId = senderName,
            receiverId = receiverName,
            response = new ChatResponse
            {
                requestType = ChatRequestType.Message,
                requestTimeId = DateTime.UtcNow
            }
        };

        ChatClientHandler.TransmissionHandler(
            chatClientMock.Object,
            clientMock.Object,
            transmission
        );
        sendMessagesStoreMock.Verify(s => s.Store(transmission), Times.Exactly(1));
        receivedMessagesStoreMock.Verify(
            s => s.Store(It.IsAny<Transmission<ChatRequest, ChatResponse>>()),
            Times.Never()
        );
        callbackMock.Verify(c => c());
    }

    [TestMethod]
    public void ReceivesAndParsesConnectedClientsList()
    {
        string receiverName = "receiver";
        var transmission = new Transmission<ChatRequest, ChatResponse>
        {
            transmissionType = TransmissionType.response,
            targetType = TargetType.client,
            receiverId = receiverName,
            response = new ChatResponse
            {
                requestType = ChatRequestType.ClientList,
                message = "[\"test\", \"other\"]"
            }
        };

        ChatClientHandler.TransmissionHandler(
            chatClientMock.Object,
            clientMock.Object,
            transmission
        );
        Assert.IsTrue(chatClientMock.Object.availableClients.Contains("test"));
        Assert.IsTrue(chatClientMock.Object.availableClients.Contains("other"));
        callbackMock.Verify(c => c());
    }
}
