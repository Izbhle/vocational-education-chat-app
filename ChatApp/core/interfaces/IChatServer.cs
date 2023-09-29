namespace ChatApp
{
    public interface IChatServer
    {
        public abstract void Start();
        public abstract ChatResponse CreateListOfClientsResponse();
    }
}
