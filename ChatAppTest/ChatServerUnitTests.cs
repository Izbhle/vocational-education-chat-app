using System.Text.Json;
using Network;
using Transmissions;

namespace ChatAppServer;

[TestClass]
public class ChatServerUnitTests
{
    private readonly Mock<INetworkServer<ChatRequest, ChatResponse>> networkServerMock = new();

    [TestMethod]
    public void MakesRequestClientListRequest()
    {
        var clientsList = new List<string> { "test1", "test2" };
        var chatServerMock = new Mock<ChatServer>(networkServerMock.Object) { CallBase = true };
        networkServerMock.Setup(s => s.GetListOfClientIds()).Returns(() => clientsList);
        var response = chatServerMock.Object.CreateListOfClientsResponse();
        Assert.IsTrue(response.message == JsonSerializer.Serialize(clientsList));
        Assert.IsTrue(response.requestType == ChatRequestType.ClientList);
    }
}
