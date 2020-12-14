using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser.ViewModels
{
    public class LogDetailsViewModel : Screen
    {
        private string test;

        public LogDetailsViewModel()
        {
        }

        public string Test
        {
            get { return test; }
            set { SetAndNotify(ref test, value); }
        }
    }
}
