using Stylet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser.ViewModels
{
    public class AboutViewModel : Screen
    {
        public void OpenLink(string link)
        {
            Process.Start("explorer", link);
        }
    }
}
