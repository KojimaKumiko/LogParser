using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace LogParser.ViewModels
{
    public class SettingsViewModel : Screen
    {
        public SettingsViewModel()
        {
            Settings = new BindableCollection<Settings>();
            _ = LoadDataFromDatabase();
        }

        public BindableCollection<Settings> Settings { get; private set; }

        public async Task SaveSettings()
        {
            using DatabaseContext context = new DatabaseContext();
            List<Settings> settings = await context.Settings.ToListAsync().ConfigureAwait(false);

            foreach (var setting in settings)
            {
                setting.Value = Settings.First(s => s.Name.Equals(setting.Name, StringComparison.OrdinalIgnoreCase)).Value;
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task LoadDataFromDatabase()
        {
            using DatabaseContext context = new DatabaseContext();
            var settings = await context.Settings.ToListAsync().ConfigureAwait(false);
            Settings.AddRange(settings);
        }
    }
}
