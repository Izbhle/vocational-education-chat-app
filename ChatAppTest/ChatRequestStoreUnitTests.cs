using Network;

namespace ChatApp;

[TestClass]
public class ChatRequestStoreUnitTests
{
    private ChatTransmission messageFromAtoB = new ChatTransmission
    {
        senderId = "A",
        receiverId = "B",
        transmissionType = TransmissionType.request,
        request = new ChatRequest
        {
            requestType = ChatRequestType.Message,
            requestTimeId = new DateTime(9000),
            message = "message"
        }
    };

    private ChatTransmission responseFromBtoA = new ChatTransmission
    {
        senderId = "B",
        receiverId = "A",
        transmissionType = TransmissionType.response,
        response = new ChatResponse
        {
            requestType = ChatRequestType.Message,
            requestTimeId = new DateTime(9000),
        }
    };

    [TestMethod]
    public void StoresReceivedMessageAndResponse()
    {
        string id = "B";
        var requestStoreMock = new Mock<ChatRequestStore>(id) { CallBase = true };
        requestStoreMock.Object.Store(messageFromAtoB);
        Assert.IsTrue(requestStoreMock.Object.requestTransmissions.ContainsKey("A"));
        Assert.IsTrue(
            requestStoreMock.Object.requestTransmissions["A"].ContainsKey(
                messageFromAtoB.request!.requestTimeId.ToString()!
            )
        );
        Assert.IsTrue(
            requestStoreMock.Object.requestTransmissions["A"][
                messageFromAtoB.request!.requestTimeId.ToString()!
            ].request == messageFromAtoB.request
        );
        requestStoreMock.Object.Store(responseFromBtoA);
        Assert.IsTrue(
            requestStoreMock.Object.requestTransmissions["A"][
                messageFromAtoB.request!.requestTimeId.ToString()!
            ].response == responseFromBtoA.response
        );
    }

    [TestMethod]
    public void StoresSendMessageAndResponse()
    {
        string id = "A";
        var requestStoreMock = new Mock<ChatRequestStore>(id) { CallBase = true };
        requestStoreMock.Object.Store(messageFromAtoB);
        Assert.IsTrue(requestStoreMock.Object.requestTransmissions.ContainsKey("B"));
        Assert.IsTrue(
            requestStoreMock.Object.requestTransmissions["B"].ContainsKey(
                messageFromAtoB.request!.requestTimeId.ToString()!
            )
        );
        Assert.IsTrue(
            requestStoreMock.Object.requestTransmissions["B"][
                messageFromAtoB.request!.requestTimeId.ToString()!
            ].request == messageFromAtoB.request
        );
        requestStoreMock.Object.Store(responseFromBtoA);
        Assert.IsTrue(
            requestStoreMock.Object.requestTransmissions["B"][
                messageFromAtoB.request!.requestTimeId.ToString()!
            ].response == responseFromBtoA.response
        );
    }
}
