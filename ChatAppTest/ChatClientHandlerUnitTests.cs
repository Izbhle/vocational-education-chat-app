using Network;
using Transmissions;

namespace ChatAppClient;

[TestClass]
public class ChatClientHandlerUnitTests
{
    private readonly Mock<INetworkClient<ChatRequest, ChatResponse>> clientMock = new();
    private readonly Mock<IChatClient> chatClientMock = new();
    private readonly Mock<IChatRequestStore> messagesStoreMock = new();
    private readonly Mock<Action> callbackMock = new();

    public ChatClientHandlerUnitTests()
    {
        chatClientMock.SetupGet(c => c.messagesStore).Returns(messagesStoreMock.Object);
        chatClientMock.SetupGet(c => c.callback).Returns(callbackMock.Object);
        chatClientMock.SetupProperty(c => c.availableClients);
    }

    [TestMethod]
    public void StoresValidReceivedMessage()
    {
        string message = "message";
        string senderName = "sender";
        string receiverName = "receiver";
        var transmission = new ChatTransmission
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
        messagesStoreMock.Verify(s => s.Store(transmission), Times.Exactly(1));
        callbackMock.Verify(c => c());
    }

    [TestMethod]
    public void StoresValidResponseMessage()
    {
        string senderName = "sender";
        string receiverName = "receiver";
        var transmission = new ChatTransmission
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
        messagesStoreMock.Verify(s => s.Store(transmission), Times.Exactly(1));
        callbackMock.Verify(c => c());
    }

    [TestMethod]
    public void ReceivesAndParsesConnectedClientsList()
    {
        string receiverName = "receiver";
        var transmission = new ChatTransmission
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
