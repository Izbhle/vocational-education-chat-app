using Network;

namespace ChatApp
{
    public class ChatRequestStore : IChatRequestStore
    {
        private readonly string storeId;

        public Dictionary<
            string,
            Dictionary<string, Transmission<ChatRequest, ChatResponse>>
        > requestTransmissions { get; }

        public ChatRequestStore(string id)
        {
            storeId = id;
            requestTransmissions =
                new Dictionary<
                    string,
                    Dictionary<string, Transmission<ChatRequest, ChatResponse>>
                >();
        }

        private Dictionary<
            string,
            Transmission<ChatRequest, ChatResponse>
        > GetTargetTransmissionDict(string receiverId, string senderId)
        {
            string targetId;
            if (senderId == storeId)
            {
                targetId = receiverId;
            }
            else
            {
                targetId = senderId;
            }
            if (!requestTransmissions.ContainsKey(targetId))
            {
                requestTransmissions.Add(
                    targetId,
                    new Dictionary<string, Transmission<ChatRequest, ChatResponse>>()
                );
            }
            return requestTransmissions[targetId];
        }

        public void Store(Transmission<ChatRequest, ChatResponse>? transmission)
        {
            if (
                transmission == null
                || transmission.receiverId == null
                || transmission.senderId == null
            )
                return;
            var targetDict = GetTargetTransmissionDict(
                transmission.receiverId,
                transmission.senderId
            );
            if (targetDict == null)
                return;
            switch (transmission.transmissionType)
            {
                case TransmissionType.request:
                    if (transmission.request?.requestTimeId == null)
                        return;
                    if (!targetDict.ContainsKey(transmission.request.requestTimeId.ToString()!))
                    {
                        targetDict.TryAdd(
                            transmission.request.requestTimeId.ToString()!,
                            new Transmission<ChatRequest, ChatResponse>
                            {
                                targetType = transmission.targetType,
                                transmissionType = transmission.transmissionType
                            }
                        );
                    }
                    targetDict[transmission.request.requestTimeId.ToString()!].request =
                        transmission.request;
                    targetDict[transmission.request.requestTimeId.ToString()!].senderId =
                        transmission.senderId;
                    targetDict[transmission.request.requestTimeId.ToString()!].receiverId =
                        transmission.receiverId;
                    break;
                case TransmissionType.response:
                    if (transmission.response?.requestTimeId == null)
                        return;
                    if (!targetDict.ContainsKey(transmission.response.requestTimeId.ToString()!))
                    {
                        targetDict.TryAdd(
                            transmission.response.requestTimeId.ToString()!,
                            new Transmission<ChatRequest, ChatResponse>
                            {
                                targetType = transmission.targetType,
                                transmissionType = transmission.transmissionType
                            }
                        );
                    }
                    targetDict[transmission.response.requestTimeId.ToString()!].response =
                        transmission.response;
                    break;
            }
        }
    }
}
