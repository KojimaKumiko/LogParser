namespace LogParser.Events
{
    public class ProgressEvent
    {
        public ProgressEvent(double progressIncrement)
        {
            ProgressIncrement = progressIncrement;
        }
        
        public double ProgressIncrement { get; set; }
    }
}