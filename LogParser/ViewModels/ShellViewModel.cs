using MaterialDesignThemes.Wpf;
using Serilog;
using Stylet;
using System;

namespace LogParser.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>
    {
        private readonly LogParserViewModel logParserViewModel;

        private readonly SettingsViewModel settingsViewModel;

        private SnackbarMessageQueue messageQueue;

        public ShellViewModel(LogParserViewModel logParserViewModel, SettingsViewModel settingsViewModel, SnackbarMessageQueue messageQueue)
        {
            this.logParserViewModel = logParserViewModel;
            this.settingsViewModel = settingsViewModel;
            this.messageQueue = messageQueue;
        }

        public SnackbarMessageQueue MessageQueue
        {
            get { return messageQueue; }
            set { SetAndNotify(ref messageQueue, value); }
        }

        public void SwitchView(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.LogParserViewModel:
                    ActivateItem(logParserViewModel);
                    break;
                case ViewType.SettingsViewModel:
                    ActivateItem(settingsViewModel);
                    break;
                default:
                    break;
            }
        }
    }
}
