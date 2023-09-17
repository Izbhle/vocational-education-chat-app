using Network;

namespace ChatApp
{
    public class ChatRequestStore : IChatRequestStore
    {
        private readonly string storeId;

        public Dictionary<
            string,
            Dictionary<DateTime, Transmission<ChatRequest, ChatResponse>>
        > requestTransmissions { get; }

        public ChatRequestStore(string id)
        {
            storeId = id;
            requestTransmissions =
                new Dictionary<
                    string,
                    Dictionary<DateTime, Transmission<ChatRequest, ChatResponse>>
                >();
        }

        private Dictionary<
            DateTime,
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
                    new Dictionary<DateTime, Transmission<ChatRequest, ChatResponse>>()
                );
            }
            return requestTransmissions[targetId];
        }

        public void Store(Transmission<ChatRequest, ChatResponse>? Transmission)
        {
            if (
                Transmission == null
                || Transmission.receiverId == null
                || Transmission.senderId == null
            )
            {
                return;
            }
            switch (Transmission.transmissionType)
            {
                case TransmissionType.request:
                    if (Transmission.request?.requestTimeId == null)
                    {
                        return;
                    }
                    GetTargetTransmissionDict(Transmission.receiverId, Transmission.senderId)
                        .TryAdd((DateTime)Transmission.request?.requestTimeId!, Transmission);
                    break;
                case TransmissionType.response:
                    if (
                        Transmission.request?.requestTimeId == null
                        || !GetTargetTransmissionDict(
                                Transmission.receiverId,
                                Transmission.senderId
                            )
                            .ContainsKey(Transmission.request.requestTimeId)
                        || GetTargetTransmissionDict(
                            Transmission.receiverId,
                            Transmission.senderId
                        )[Transmission.request.requestTimeId] == null
                    )
                    {
                        return;
                    }
                    GetTargetTransmissionDict(Transmission.receiverId, Transmission.senderId)[
                        Transmission.request.requestTimeId
                    ].response = Transmission.response;
                    break;
            }
        }
    }
}
