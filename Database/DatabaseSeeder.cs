using Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;

namespace Database
{
    public static class DatabaseSeeder
    {
        public static void Seed(DatabaseContext context, bool isDev)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            bool shouldSeed = false;

            if (isDev)
            {
                //context.Database.EnsureDeleted();
                shouldSeed = context.Database.EnsureCreated();
            }
            else
            {
                context.Database.Migrate();
                shouldSeed = !SettingsManager.GetDatabaseSeeded(context);
            }

            if (shouldSeed)
            {
                context.Settings.Add(new Setting { Name = SettingsManager.DpsReport, Value = "False" });
                context.Settings.Add(new Setting { Name = SettingsManager.UserToken, Value = string.Empty });
                context.Settings.Add(new Setting { Name = SettingsManager.WebhookUrl, Value = string.Empty });
                context.Settings.Add(new Setting { Name = SettingsManager.WebhookName, Value = string.Empty });
                context.Settings.Add(new Setting { Name = SettingsManager.PostDiscord, Value = "False" });
                context.Settings.Add(new Setting { Name = SettingsManager.Seeded, Value = "True" });
                context.SaveChanges();
            }
        }
    }
}
