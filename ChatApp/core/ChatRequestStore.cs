using Network;

namespace ChatApp
{
    public class ChatRequestStore
    {
        public readonly Dictionary<string, Dictionary<
            DateTime,
            Transmission<ChatRequest, ChatResponse>
        >> requestTransmissions;

        public ChatRequestStore()
        {
            requestTransmissions = new Dictionary<string, Dictionary<
            DateTime,
            Transmission<ChatRequest, ChatResponse>
        >>();
        }

        private Dictionary<DateTime, Transmission<ChatRequest, ChatResponse>> getReceiverTransmissionDict(string receiverId)
        {
            if (!requestTransmissions.ContainsKey(receiverId))
            {
                requestTransmissions.Add(receiverId, new Dictionary<DateTime, Transmission<ChatRequest, ChatResponse>>());
            }
            return requestTransmissions[receiverId];
        }

        public void Store(Transmission<ChatRequest, ChatResponse>? Transmission)
        {
            if (Transmission == null || Transmission.receiverId == null || Transmission.senderId == null)
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
                    getReceiverTransmissionDict(Transmission.senderId).Add((DateTime)Transmission.request?.requestTimeId!, Transmission);
                    break;
                case TransmissionType.response:
                    if (
                        Transmission.request?.requestTimeId == null ||
                        !getReceiverTransmissionDict(Transmission.receiverId).ContainsKey(Transmission.request.requestTimeId)
                        || getReceiverTransmissionDict(Transmission.receiverId)[Transmission.request.requestTimeId] == null
                    )
                    {
                        return;
                    }
                    getReceiverTransmissionDict(Transmission.receiverId)[Transmission.request.requestTimeId].response = Transmission.response;
                    break;
            }
        }
    }
}