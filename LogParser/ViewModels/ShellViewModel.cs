using MaterialDesignThemes.Wpf;
using Stylet;
using System.Reflection;

namespace LogParser.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly LogParserViewModel logParserViewModel;

        private readonly SettingsViewModel settingsViewModel;

        private readonly AboutViewModel aboutViewModel;

        private SnackbarMessageQueue messageQueue;

        private string version;

        public ShellViewModel(LogParserViewModel logParserViewModel, SettingsViewModel settingsViewModel, AboutViewModel aboutViewModel, SnackbarMessageQueue messageQueue)
        {
            this.logParserViewModel = logParserViewModel;
            this.settingsViewModel = settingsViewModel;
            this.aboutViewModel = aboutViewModel;
            this.messageQueue = messageQueue;

            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Version = $"Ver. {assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
        }

        public SnackbarMessageQueue MessageQueue
        {
            get { return messageQueue; }
            set { SetAndNotify(ref messageQueue, value); }
        }

        public string Version
        {
            get { return version; }
            set { SetAndNotify(ref version, value); }
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
                case ViewType.AboutViewModel:
                    ActivateItem(aboutViewModel);
                    break;
                default:
                    break;
            }
        }
    }
}
