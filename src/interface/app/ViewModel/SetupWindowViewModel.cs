namespace ViewModels
{
    public class SetupWindowViewModel : ViewModelBase
    {
        // Add our SimpleViewModel.
        // Note: We need at least a get-accessor for our Properties.
        public StartClientModel StartClientModel { get; } = new StartClientModel();

        // Add our ReactiveViewModel
        public StartServerModel StartServerModel { get; } = new StartServerModel();
    }
}
