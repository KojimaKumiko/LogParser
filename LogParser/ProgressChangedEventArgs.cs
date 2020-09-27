using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser
{
    public class ProgressChangedEventArgs : EventArgs
    {
        public ProgressChangedEventArgs(int progress)
        {
            Progress = progress;
        }

        public int Progress { get; }
    }
}
