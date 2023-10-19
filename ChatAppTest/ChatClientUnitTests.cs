using Network;
using Transmissions;

namespace ChatAppClient;

[TestClass]
public class ChatClientUnitTests
{
    private readonly Mock<INetworkClient<ChatRequest, ChatResponse>> networkClientMock = new();

    [TestMethod]
    public void SendsRequestClientListRequest()
    {
        string id = "test";
        var chatClientMock = new Mock<ChatClient>(id, networkClientMock.Object, () => { })
        {
            CallBase = true
        };
        chatClientMock.Object.Start();
        chatClientMock.Object.RequestClientList();
        networkClientMock.Verify(
            c => c.SendServerRequest(ChatClientTransmissions.getClientListRequest)
        );
    }

    [TestMethod]
    public void SendsMessageRequest()
    {
        string id = "test";
        string target = "target";
        string message = "message";
        var chatClientMock = new Mock<ChatClient>(id, networkClientMock.Object, () => { })
        {
            CallBase = true
        };
        chatClientMock.Object.Start();
        chatClientMock.Object.SendMessage(target, message);
        networkClientMock.Verify(
            c =>
                c.SendClientRequest(
                    target,
                    It.Is<ChatRequest>(
                        (r) => r.requestType == ChatRequestType.Message && r.message == message
                    )
                )
        );
    }
}
