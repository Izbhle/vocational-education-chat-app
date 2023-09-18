using Network;

namespace ChatApp;

[TestClass]
public class ChatServerHandlerUnitTests
{
    private readonly Mock<INetworkServer<ChatRequest, ChatResponse>> serverMock = new();
    private readonly Mock<INetworkClient<ChatRequest, ChatResponse>> clientMock = new();
    private readonly Mock<IChatServer> chatServerMock = new();

    [TestMethod]
    public void RegistersClient()
    {
        string clientName = "test";
        var transmission = new Transmission<ChatRequest, ChatResponse>
        {
            transmissionType = TransmissionType.request,
            targetType = TargetType.server,
            request = new ChatRequest
            {
                requestType = ChatRequestType.RegisterClient,
                message = clientName
            }
        };

        ChatServerHandler.TransmissionHandler(
            chatServerMock.Object,
            serverMock.Object,
            clientMock.Object,
            transmission
        );
        serverMock.Verify(
            s => s.RegisterClientAction(clientMock.Object, clientName),
            Times.Exactly(1)
        );
    }

    [TestMethod]
    public void DisconnectsClient()
    {
        string clientName = "test";
        clientMock.SetupProperty(c => c.Id, clientName);
        var transmission = new Transmission<ChatRequest, ChatResponse>
        {
            transmissionType = TransmissionType.request,
            targetType = TargetType.server,
            request = new ChatRequest { requestType = ChatRequestType.DisconnectClient, }
        };

        ChatServerHandler.TransmissionHandler(
            chatServerMock.Object,
            serverMock.Object,
            clientMock.Object,
            transmission
        );
        serverMock.Verify(s => s.DisconnectClientAction(clientName), Times.Exactly(1));
        clientMock.Verify(c => c.Dispose(), Times.Exactly(1));
    }

    [TestMethod]
    public void RelaysClientTransmission()
    {
        string clientName = "test";
        string targetName = "target";
        string message = "message";
        var transmission = new Transmission<ChatRequest, ChatResponse>
        {
            transmissionType = TransmissionType.request,
            targetType = TargetType.client,
            senderId = clientName,
            receiverId = targetName,
            request = new ChatRequest
            {
                requestType = ChatRequestType.Message,
                message = message,
                requestTimeId = DateTime.UtcNow
            }
        };

        ChatServerHandler.TransmissionHandler(
            chatServerMock.Object,
            serverMock.Object,
            clientMock.Object,
            transmission
        );
        serverMock.Verify(s => s.TrySendTransmission(targetName, transmission), Times.Exactly(1));
    }

    [TestMethod]
    public void CreatesReceiverUnknownResponse()
    {
        string clientName = "test";
        string targetName = "target";
        string message = "message";
        var transmission = new Transmission<ChatRequest, ChatResponse>
        {
            transmissionType = TransmissionType.request,
            targetType = TargetType.client,
            senderId = clientName,
            receiverId = targetName,
            request = new ChatRequest
            {
                requestType = ChatRequestType.Message,
                message = message,
                requestTimeId = DateTime.UtcNow
            }
        };
        serverMock.Setup(s => s.TrySendTransmission(targetName, transmission)).Returns(false);

        ChatServerHandler.TransmissionHandler(
            chatServerMock.Object,
            serverMock.Object,
            clientMock.Object,
            transmission
        );
        clientMock.Verify(
            c => c.SendResponse(transmission, ChatServerHandler.malformedRequestResponse),
            Times.Exactly(1)
        );
    }

    [TestMethod]
    public void ReturnsListOfClients()
    {
        string clientName = "test";
        var transmission = new Transmission<ChatRequest, ChatResponse>
        {
            transmissionType = TransmissionType.request,
            targetType = TargetType.server,
            senderId = clientName,
            request = new ChatRequest { requestType = ChatRequestType.ClientList, }
        };

        var response = new ChatResponse
        {
            requestType = ChatRequestType.ClientList,
            message = "[\"test\", \"other\"]"
        };

        chatServerMock.Setup(s => s.CreateListOfClientsResponse()).Returns(response);
        ChatServerHandler.TransmissionHandler(
            chatServerMock.Object,
            serverMock.Object,
            clientMock.Object,
            transmission
        );
        chatServerMock.Verify(s => s.CreateListOfClientsResponse(), Times.Exactly(1));
        clientMock.Verify(s => s.SendResponse(transmission, response));
    }
}
