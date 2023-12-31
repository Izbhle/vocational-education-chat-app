using Transmissions;

namespace ChatAppClient
{
    public class ChatRequestStore : IChatRequestStore
    {
        private readonly string storeId;

        public Dictionary<
            string,
            Dictionary<string, ChatTransmission>
        > RequestTransmissions { get; }

        public ChatRequestStore(string id)
        {
            storeId = id;
            RequestTransmissions = new Dictionary<string, Dictionary<string, ChatTransmission>>();
        }

        private Dictionary<string, ChatTransmission> GetTargetTransmissionDict(
            string receiverId,
            string senderId
        )
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
            if (!RequestTransmissions.ContainsKey(targetId))
            {
                RequestTransmissions.Add(targetId, new Dictionary<string, ChatTransmission>());
            }
            return RequestTransmissions[targetId];
        }

        public void Store(ITransmission<ChatRequest, ChatResponse>? transmission)
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
                            new ChatTransmission
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
                        return;
                    targetDict[transmission.response.requestTimeId.ToString()!].response =
                        transmission.response;
                    break;
            }
        }
    }
}
