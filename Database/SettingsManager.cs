using Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    /// <summary>
    /// Static class to manage the settings of the application.
    /// </summary>
    public static class SettingsManager
    {
        public static string DpsReport => "UploadToDpsReport";

        public static string UserToken => "UserToken";

        public static string WebhookUrl => "DiscordWebhookUrl";

        public static string WebhookName => "DiscordWebhookName";

        public static string PostDiscord => "PostToDiscord";

        public static async Task<List<Setting>> GetSettings(DatabaseContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            List<Setting> settings = await dbContext.Settings.ToListAsync().ConfigureAwait(false);
            return settings;
        }

        public static async Task<bool> GetDpsReportUploadAsync(DatabaseContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            Setting setting = await dbContext.Settings.SingleAsync(s => s.Name == DpsReport).ConfigureAwait(false);

            return Convert.ToBoolean(setting.Value, CultureInfo.InvariantCulture);
        }

        public static async Task<string> GetUserTokenAsync(DatabaseContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            Setting setting = await dbContext.Settings.SingleAsync(s => s.Name == UserToken).ConfigureAwait(false);

            return setting.Value;
        }

        public static async Task<string> GetDiscordWebhookUrl(DatabaseContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            Setting setting = await dbContext.Settings.SingleAsync(s => s.Name == WebhookUrl).ConfigureAwait(false);

            return setting.Value;
        }

        public static async Task<string> GetDiscordWebhookName(DatabaseContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            Setting setting = await dbContext.Settings.SingleAsync(s => s.Name == WebhookName).ConfigureAwait(false);

            return setting.Value;
        }

        public static async Task<bool> GetPostToDiscord(DatabaseContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            Setting setting = await dbContext.Settings.SingleAsync(s => s.Name == PostDiscord).ConfigureAwait(false);

            return Convert.ToBoolean(setting.Value, CultureInfo.InvariantCulture);
        }

        public static async Task UpdateSetting(DatabaseContext dbContext, string value, string settingName)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (string.IsNullOrWhiteSpace(settingName))
            {
                throw new ArgumentNullException(nameof(settingName));
            }

            Setting setting = await dbContext.Settings.SingleOrDefaultAsync(s => s.Name == settingName).ConfigureAwait(false);

            if (setting == null)
            {
                throw new ArgumentException($"The provided setting name does not exist: {settingName}");
            }

            setting.Value = value;
        }
    }
}
