using Database;
using LogParser.Controller;
using Stylet;
using System;
using System.Threading.Tasks;

namespace LogParser.ViewModels
{
    public class SettingsViewModel : Screen
    {
        private readonly DatabaseContext dbContext;

        private readonly DpsReportController dpsReportController;

        private bool uploadDspReport;

        private bool postToDiscord;

        private string userToken;

        private string webhookUrl;

        private string webhookName;

        public SettingsViewModel(DatabaseContext dbContext, DpsReportController dpsReportController)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.dpsReportController = dpsReportController ?? throw new ArgumentNullException(nameof(dpsReportController));

            _ = LoadDataFromDatabase();
        }

        public bool UploadDpsReport
        {
            get { return uploadDspReport; }
            set { SetAndNotify(ref uploadDspReport, value); }
        }

        public bool PostToDiscord
        {
            get { return postToDiscord; }
            set { SetAndNotify(ref postToDiscord, value); }
        }

        public string UserToken
        {
            get { return userToken; }
            set { SetAndNotify(ref userToken, value); }
        }

        public string WebhookUrl
        {
            get { return webhookUrl; }
            set { SetAndNotify(ref webhookUrl, value); }
        }

        public string WebhookName
        {
            get { return webhookName; }
            set { SetAndNotify(ref webhookName, value); }
        }

        public async Task GenerateToken()
        {
            UserToken = await dpsReportController.GetUserToken().ConfigureAwait(true);
        }

        public async Task SaveSettings()
        {
            await SettingsManager.UpdateSetting(dbContext, UploadDpsReport.ToString(), SettingsManager.DpsReport).ConfigureAwait(true);
            await SettingsManager.UpdateSetting(dbContext, PostToDiscord.ToString(), SettingsManager.PostDiscord).ConfigureAwait(true);
            await SettingsManager.UpdateSetting(dbContext, UserToken, SettingsManager.UserToken).ConfigureAwait(true);
            await SettingsManager.UpdateSetting(dbContext, WebhookUrl, SettingsManager.WebhookUrl).ConfigureAwait(true);
            await SettingsManager.UpdateSetting(dbContext, WebhookName, SettingsManager.WebhookName).ConfigureAwait(true);

            await dbContext.SaveChangesAsync().ConfigureAwait(true);
        }

        private async Task LoadDataFromDatabase()
        {
            UploadDpsReport = await SettingsManager.GetDpsReportUploadAsync(dbContext).ConfigureAwait(true);
            UserToken = await SettingsManager.GetUserTokenAsync(dbContext).ConfigureAwait(true);
            WebhookUrl = await SettingsManager.GetDiscordWebhookUrl(dbContext).ConfigureAwait(true);
            WebhookName = await SettingsManager.GetDiscordWebhookName(dbContext).ConfigureAwait(true);
            PostToDiscord = await SettingsManager.GetPostToDiscord(dbContext).ConfigureAwait(true);
        }
    }
}
