using Serilog;
using Stylet;
using System;

namespace LogParser.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>
    {
        private readonly Func<LogParserViewModel> logParserFunc;

        private readonly Func<SettingsViewModel> settingsFunc;

        public ShellViewModel(Func<LogParserViewModel> logParserFunc, Func<SettingsViewModel> settingsFunc)
        {
            Log.Debug("ShellViewModel constructor called.");

            this.logParserFunc = logParserFunc;
            this.settingsFunc = settingsFunc;
        }

        public void SwitchView(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.LogParserViewModel:
                    ActivateItem(logParserFunc());
                    break;
                case ViewType.SettingsViewModel:
                    ActivateItem(settingsFunc());
                    break;
                default:
                    break;
            }
        }
    }
}
