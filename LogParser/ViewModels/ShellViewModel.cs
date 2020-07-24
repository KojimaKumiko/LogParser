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

        public void SwitchView(ViewTypes viewType)
        {
            switch (viewType)
            {
                case ViewTypes.TestViewModel:
                    ActivateItem(new TestViewModel());
                    break;
                default:
                    break;
            }
        }
    }
}
