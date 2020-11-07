using Database;
using Microsoft.Win32;
using Stylet;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Media;
using System.Reflection;

namespace LogParser.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>
    {
        private readonly Func<LogParserViewModel> logParserFunc;

        private readonly Func<SettingsViewModel> settingsFunc;

        public ShellViewModel(Func<LogParserViewModel> logParserFunc, Func<SettingsViewModel> settingsFunc)
        {
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
