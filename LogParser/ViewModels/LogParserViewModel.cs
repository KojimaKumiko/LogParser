using Database;
using Database.Models;
using LogParser.Controller;
using LogParser.Models;
using LogParser.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestEase;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace LogParser.ViewModels
{
    public class LogParserViewModel : Screen
    {
        private const string DialogIdentifier = "RootDialogHost";

        private readonly string clearFilterText = " --- NONE --- ";

        private string selectedBossFilter;

        private string displayText;

        private string eliteInsightsVersion;

        private string updateEliteInsightsContent;

        private int progress;

        private bool showProgressBar;

        private bool installed;

        public LogParserViewModel()
        {
            LogFiles = new BindableCollection<ParsedLogFile>();
            FilesToParse = new BindableCollection<string>();
            BossNameFilters = new BindableCollection<string>()
            {
                clearFilterText
            };

            LogFiles.CollectionChanged += LogFilesCollectionChanged;
            FilesToParse.CollectionChanged += (o, e) => NotifyOfPropertyChange(nameof(CanParseFiles));

            var fileVersion = ParseController.GetFileVersionInfo();
            if (fileVersion == null)
            {
                EliteInsightsVersion = "Not Installed";
                UpdateEliteInsightsContent = "Install Elite Insights";
                installed = false;
            }
            else
            {
                EliteInsightsVersion = fileVersion.FileVersion;
                UpdateEliteInsightsContent = "Update Elite Insights";
                installed = true;
            }

            //OpenLinkCommand = new RelayCommand<ParsedLogFile>((logFile) => OpenLink(logFile, false));

            _ = LoadDataFromDatabase();
        }

        public static string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string BaseLogResult => "LogResult";

        public static string LogResultPath => Path.Combine(AssemblyLocation, BaseLogResult);

        public BindableCollection<ParsedLogFile> LogFiles { get; private set; }

        public BindableCollection<string> BossNameFilters { get; private set; }

        public BindableCollection<string> FilesToParse { get; private set; }

        public ICommand OpenLinkCommand { get; set; }

        public string DisplayText
        {
            get { return displayText; }
            set { SetAndNotify(ref displayText, value); }
        }

        public string SelectedBossFilter
        {
            get { return selectedBossFilter; }
            set { SetAndNotify(ref selectedBossFilter, value); }
        }

        public string EliteInsightsVersion
        {
            get { return eliteInsightsVersion; }
            set { SetAndNotify(ref eliteInsightsVersion, value); }
        }

        public string UpdateEliteInsightsContent
        {
            get { return updateEliteInsightsContent; }
            set { SetAndNotify(ref updateEliteInsightsContent, value); }
        }

        public int Progress
        {
            get { return progress; }
            set { SetAndNotify(ref progress, value); }
        }

        public bool ShowProgressBar
        {
            get { return showProgressBar; }
            set { SetAndNotify(ref showProgressBar, value); }
        }

        public bool CanParseFiles
        {
            get { return FilesToParse != null && FilesToParse.Count > 0; }
        }

        public void SetFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = ".evtc, .zevtc|*.evtc;*.zevtc"
            };
            bool result = (bool)fileDialog.ShowDialog();

            if (result)
            {
                FilesToParse.AddRange(fileDialog.FileNames);
                FilesToParse.Refresh();
            }
        }

        public async Task ParseFiles()
        {
            Debug.WriteLine("Parse Files.");

            using var dbContext = new DatabaseContext();
            bool upload = await SettingsManager.GetDpsReportUploadAsync(dbContext).ConfigureAwait(true);
            string userToken = await SettingsManager.GetUserTokenAsync(dbContext).ConfigureAwait(true);

            List<ParsedLogFile> parsedLogs = new List<ParsedLogFile>();
            Task<DPSReport> uploadTask = null;
            string htmlPath = Path.Combine(ParseController.AssemblyLocation, "Logs");

            foreach (string file in FilesToParse)
            {
                if (upload)
                {
                    // upload the file...
                    uploadTask = DpsReportController.UploadToDpsReport(file, userToken);
                }

                ParsedLogFile logFile = ParseController.ParseSingleFile(file, htmlPath);

                ParseController.ClearLogFolder();

                if (uploadTask != null)
                {
                    DPSReport report = await uploadTask.ConfigureAwait(true);

                    if (report != null)
                    {
                        logFile.DpsReportLink = report.PermaLink;
                    }
                }

                parsedLogs.Add(logFile);
            }

            dbContext.ParsedLogFiles.AddRange(parsedLogs);

            await dbContext.SaveChangesAsync().ConfigureAwait(true);
            FilesToParse.Clear();

            await LoadDataFromDatabase().ConfigureAwait(true);

            Debug.WriteLine("Done");
        }

        public async Task UpdateEliteInsights()
        {
            await ParseController.InstallEliteInsights().ConfigureAwait(true);

            var fileVersion = ParseController.GetFileVersionInfo();
            EliteInsightsVersion = fileVersion.FileVersion;
            UpdateEliteInsightsContent = "Update Elite Insights";

            string message = installed ? "Successfully updated Elite Insights!" : "Successfully installed Elite Insights!";
            var messageModel = new MessageViewModel(message);
            await MaterialDesignThemes.Wpf.DialogHost.Show(messageModel, DialogIdentifier).ConfigureAwait(true);

            installed = true;
        }

        public async void BossFilterChanged(object sender, SelectionChangedEventArgs eventArgs)
        {
            if (string.IsNullOrWhiteSpace(selectedBossFilter) || eventArgs == null)
            {
                return;
            }

            using DatabaseContext db = new DatabaseContext();

            var logFiles = db.ParsedLogFiles.AsQueryable();

            if (SelectedBossFilter.Equals(clearFilterText, StringComparison.InvariantCultureIgnoreCase))
            {
                SelectedBossFilter = null;
            }
            else
            {
                logFiles = logFiles.Where(l => l.BossName == SelectedBossFilter);
            }

            var filteredLogFiles = await logFiles.ToListAsync().ConfigureAwait(true);

            LogFiles.Clear();
            LogFiles.AddRange(filteredLogFiles);
        }

        public void CheckVersion()
        {
            FileVersionInfo fileVersionInfo = ParseController.GetFileVersionInfo();
            if (fileVersionInfo == null)
            {
                DisplayText = "EI is not installed";
            }

            DisplayText = fileVersionInfo.ToString();
        }

        public void OpenLink(ParsedLogFile logFile)
        {
            if (logFile == null)
            {
                throw new ArgumentNullException(nameof(logFile));
            }

            Process.Start("explorer", logFile.DpsReportLink);
        }

        private async Task LoadDataFromDatabase()
        {
            using DatabaseContext db = new DatabaseContext();
            List<ParsedLogFile> parsedLogs = await db.ParsedLogFiles.ToListAsync().ConfigureAwait(true);
            LogFiles.Clear();
            LogFiles.AddRange(parsedLogs);
        }

        private void LogFilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (LogFiles == null || LogFiles.Count <= 0)
            {
                return;
            }

            var bossNames = LogFiles.Select(l => l.BossName).Distinct().Except(BossNameFilters).ToList();
            BossNameFilters.AddRange(bossNames);
        }

        private void HandleProgressChangedEvent(object sender, ProgressChangedEventArgs eventArgs)
        {
            Progress = eventArgs.Progress;
        }
    }
}
