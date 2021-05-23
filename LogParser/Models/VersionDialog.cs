using LogParser.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class VersionDialog : IMessageDialog
    {
        public VersionDialog(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public bool NewVersionAvailable => !string.IsNullOrWhiteSpace(Message);
    }
}
