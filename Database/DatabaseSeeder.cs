using Database.Models;
using Database.Models.Enums;
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
                context.Settings.Add(new Settings { Name = SettingManager.DpsReport, Value = "False", DisplayOrder = 1, SettingsType = SettingsType.Boolean });
                context.SaveChanges();
            }
        }
    }
}
