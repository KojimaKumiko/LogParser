using System;
using System.Collections.Generic;
using System.Text;

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
