using Database.Models;
using System;

namespace Database
{
    public static class DatabaseSeeder
    {
        public static void Seed(DatabaseContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            bool shouldSeed = false;

#if DEBUG
            //context.Database.EnsureDeleted();
            shouldSeed = context.Database.EnsureCreated();
#else
            context.Database.Migrate();
#endif

            if (shouldSeed)
            {
                context.Settings.Add(new Setting { Name = SettingsManager.DpsReport, Value = "False" });
                context.Settings.Add(new Setting { Name = SettingsManager.UserToken, Value = string.Empty });
                context.SaveChanges();
            }
        }
    }
}
