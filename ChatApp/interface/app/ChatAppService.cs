namespace ChatApp
{
    public class ChatAppService
    {
        public ChatClient? Client { get; set; }
        public ChatServer? Server { get; set; }
        public string Name { get; set; } = "Chose your Name";
        public List<Action> OnTransmissionActions { get; set; } = new List<Action>();

        public void RunOnTransmissionActions()
        {
            OnTransmissionActions.ForEach(a => a());
        }

        public List<Action> OnStartupActions { get; set; } = new List<Action>();

        public void RunOnStartupActions()
        {
            OnStartupActions.ForEach(a => a());
        }

        public List<Action> OnExitActions { get; set; } = new List<Action>();

        public void Exit()
        {
            OnStartupActions = new List<Action>();
            OnTransmissionActions = new List<Action>();
            Client?.Dispose();
            Thread.Sleep(200);
            Server?.Dispose();
            Thread.Sleep(200);
            OnExitActions.ForEach(a => a());
        }
    }
}
