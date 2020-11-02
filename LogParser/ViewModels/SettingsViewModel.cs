using Database;
using Stylet;
using System.Threading.Tasks;

namespace LogParser.ViewModels
{
    public class SettingsViewModel : Screen
    {
        private bool uploadDspReport;

        private string userToken;

        public SettingsViewModel()
        {
            _ = LoadDataFromDatabase();
        }

        public bool UploadDpsReport
        {
            get { return uploadDspReport; }
            set { SetAndNotify(ref uploadDspReport, value); }
        }

        public string UserToken
        {
            get { return userToken; }
            set { SetAndNotify(ref userToken, value); }
        }

        public async Task SaveSettings()
        {
            using DatabaseContext context = new DatabaseContext();

            await SettingsManager.UpdateSetting(context, UploadDpsReport.ToString(), SettingsManager.DpsReport).ConfigureAwait(true);
            await SettingsManager.UpdateSetting(context, UserToken, SettingsManager.UserToken).ConfigureAwait(true);

            await context.SaveChangesAsync().ConfigureAwait(true);
        }

        private async Task LoadDataFromDatabase()
        {
            using DatabaseContext context = new DatabaseContext();

            UploadDpsReport = await SettingsManager.GetDpsReportUploadAsync(context).ConfigureAwait(true);
            UserToken = await SettingsManager.GetUserTokenAsync(context).ConfigureAwait(true);
        }
    }
}
