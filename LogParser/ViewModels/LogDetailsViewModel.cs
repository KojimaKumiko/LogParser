using LogParser.Events;
using Stylet;
using System;

namespace LogParser.ViewModels
{
    public class LogDetailsViewModel : Screen, IHandle<LogSelectedEvent>
    {
        private readonly IEventAggregator eventAggregator;

        public LogDetailsViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "Details";

            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            this.eventAggregator.Subscribe(this);
        }

        public void Handle(LogSelectedEvent eventArgs)
        {
        }
    }
}
