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
        public ShellViewModel()
        {
        }

        public void SwitchView(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.LogParserViewModel:
                    ActivateItem(new LogParserViewModel());
                    break;
                case ViewType.SettingsViewModel:
                    ActivateItem(new SettingsViewModel());
                    break;
                default:
                    break;
            }
        }
    }
}
