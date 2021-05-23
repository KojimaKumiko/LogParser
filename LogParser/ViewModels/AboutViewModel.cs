using LogParser.Licenses;
using LogParser.Models.Interfaces;
using MaterialDesignThemes.Wpf;
using Stylet;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LogParser.ViewModels
{
    public class AboutViewModel : Screen
    {
        private const string DialogIdentifier = "RootDialogHost";
        private readonly ILicense logParserLicense;

        public AboutViewModel()
        {
            logParserLicense = new LogParserLicense();
        }

        public void OpenLink(string link)
        {
            Helper.OpenLink(link);
        }

        public async Task ShowLicense()
        {
            await DialogHost.Show(logParserLicense, DialogIdentifier).ConfigureAwait(true);
        }

        public async Task ShowThirdPartyLicenses()
        {
            await DialogHost.Show(null, DialogIdentifier).ConfigureAwait(true);
        }
    }
}
