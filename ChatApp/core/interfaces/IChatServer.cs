namespace ChatApp
{
    public interface IChatServer
    {
        public abstract void Start();
        public void Dispose();
        public abstract ChatResponse CreateListOfClientsResponse();
    }
}
