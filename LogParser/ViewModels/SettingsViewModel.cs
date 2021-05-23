using Database;
using LogParser.Models;
using LogParser.Models.Interfaces;
using LogParser.Services;
using MaterialDesignThemes.Wpf;
using RestEase;
using Stylet;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LogParser.ViewModels
{
    public class SettingsViewModel : Screen
    {
        private const string DialogIdentifier = "RootDialogHost";

        private readonly DatabaseContext dbContext;

        private readonly DpsReportService dpsReportService;

        private bool uploadDspReport;

        private bool postToDiscord;

        private string userToken;

        private string webhookUrl;

        private string webhookName;

        private SnackbarMessageQueue messageQueue;

        public SettingsViewModel(DatabaseContext dbContext, DpsReportService dpsReportService, SnackbarMessageQueue messageQueue)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.dpsReportService = dpsReportService ?? throw new ArgumentNullException(nameof(dpsReportService));
            this.messageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));

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
            UserToken = await dpsReportService.GetUserToken().ConfigureAwait(true);
        }

        public async Task SaveSettings()
        {
            await SettingsManager.UpdateSetting(dbContext, UploadDpsReport.ToString(), SettingsManager.DpsReport).ConfigureAwait(true);
            await SettingsManager.UpdateSetting(dbContext, PostToDiscord.ToString(), SettingsManager.PostDiscord).ConfigureAwait(true);
            await SettingsManager.UpdateSetting(dbContext, UserToken, SettingsManager.UserToken).ConfigureAwait(true);
            await SettingsManager.UpdateSetting(dbContext, WebhookUrl, SettingsManager.WebhookUrl).ConfigureAwait(true);
            await SettingsManager.UpdateSetting(dbContext, WebhookName, SettingsManager.WebhookName).ConfigureAwait(true);

            await dbContext.SaveChangesAsync().ConfigureAwait(true);

            messageQueue.Enqueue("Settings succesfully saved.");
        }

        public async Task CheckVersion()
        {
            string link = await Helper.CheckForNewVersion();
            var versionDialog = new VersionDialog(link);
            await DialogHost.Show(versionDialog, DialogIdentifier).ConfigureAwait(true);
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
