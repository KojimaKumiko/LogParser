using System.Collections.Generic;

namespace LogParser.Events
{
    public class FilesAddedEvent
    {
        public FilesAddedEvent(IList<string> files)
        {
            Files = files;
        }

        public IList<string> Files { get; set; }
    }
}