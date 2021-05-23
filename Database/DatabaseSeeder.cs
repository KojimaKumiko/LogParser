using Database.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Database
{
    public static class DatabaseSeeder
    {
        public static void Seed(DatabaseContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            context.Database.Migrate();
            bool shouldSeed = !SettingsManager.GetDatabaseSeeded(context);

            if (shouldSeed)
            {
                context.Settings.Add(new Setting { Name = SettingsManager.DpsReport, Value = "False" });
                context.Settings.Add(new Setting { Name = SettingsManager.UserToken, Value = string.Empty });
                context.Settings.Add(new Setting { Name = SettingsManager.WebhookUrl, Value = string.Empty });
                context.Settings.Add(new Setting { Name = SettingsManager.WebhookName, Value = string.Empty });
                context.Settings.Add(new Setting { Name = SettingsManager.PostDiscord, Value = "False" });
                context.Settings.Add(new Setting { Name = SettingsManager.UpdateCheck, Value = string.Empty });
                context.Settings.Add(new Setting { Name = SettingsManager.Seeded, Value = "True" });
                context.SaveChanges();
            }
        }
    }
}
