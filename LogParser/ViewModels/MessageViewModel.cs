namespace LogParser.ViewModels
{
    public class MessageViewModel
    {
        public MessageViewModel(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
