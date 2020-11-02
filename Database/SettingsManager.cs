using Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public static async Task<List<Setting>> GetSettings(DatabaseContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            List<Setting> settings = await context.Settings.ToListAsync().ConfigureAwait(false);
            return settings;
        }

        public static async Task<bool> GetDpsReportUploadAsync(DatabaseContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Setting setting = await context.Settings.SingleAsync(s => s.Name == DpsReport).ConfigureAwait(false);

            return Convert.ToBoolean(setting.Value, CultureInfo.InvariantCulture);
        }

        public static async Task<string> GetUserTokenAsync(DatabaseContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Setting setting = await context.Settings.SingleAsync(s => s.Name == UserToken).ConfigureAwait(false);

            return setting.Value;
        }

        public static async Task UpdateSetting(DatabaseContext context, string value, string settingName)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (string.IsNullOrWhiteSpace(settingName))
            {
                throw new ArgumentNullException(nameof(settingName));
            }

            Setting setting = await context.Settings.SingleOrDefaultAsync(s => s.Name == settingName).ConfigureAwait(false);

            if (setting == null)
            {
                throw new ArgumentException($"The provided setting name does not exist: {settingName}");
            }

            setting.Value = value;
        }
    }
}
