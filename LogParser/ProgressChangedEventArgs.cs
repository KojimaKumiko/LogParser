using System;

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
