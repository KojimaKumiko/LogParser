using LogParser.Models.Interfaces;

namespace LogParser.Models
{
    public class MessageDialog : IMessageDialog
    {
        public MessageDialog(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
