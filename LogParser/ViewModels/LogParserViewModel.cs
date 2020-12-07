using Database;
using Database.Models;
using LogParser.Services;
using LogParser.Models;
using LogParser.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Serilog;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace LogParser.ViewModels
{
    public class LogParserViewModel : Screen
    {
        private const string DialogIdentifier = "RootDialogHost";

        private readonly DatabaseContext dbContext;

        private readonly IParseService parseService;

        private readonly DpsReportController dpsReportController;

        private readonly IDiscordService discordService;

        private readonly string clearFilterText = " --- NONE --- ";

        private string selectedBossFilter;

        private string eliteInsightsVersion;

        private string updateEliteInsightsContent;

        private double progress;

        private bool showProgressBar;

        private bool installed;

        private bool isLoadingData;

        private SnackbarMessageQueue messageQueue;

        public LogParserViewModel(DatabaseContext dbContext, IParseService parseService, DpsReportController dpsReportController, IDiscordService discordService, SnackbarMessageQueue messageQueue)
        {
            Log.Debug("LogParserViewModel constructor called.");

            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.parseService = parseService ?? throw new ArgumentNullException(nameof(parseService));
            this.discordService = discordService ?? throw new ArgumentNullException(nameof(discordService));
            this.dpsReportController = dpsReportController ?? throw new ArgumentNullException(nameof(dpsReportController));
            this.messageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));

            LogFiles = new BindableCollection<ParsedLogFile>();
            FilesToParse = new BindableCollection<string>();
            BossNameFilters = new BindableCollection<string>()
            {
                clearFilterText
            };

            LogFiles.CollectionChanged += LogFilesCollectionChanged;
            FilesToParse.CollectionChanged += (o, e) => NotifyOfPropertyChange(nameof(CanParseFiles));

            string fileVersion = parseService.GetFileVersionInfo();
            if (string.IsNullOrWhiteSpace(fileVersion))
            {
                EliteInsightsVersion = Resource.EliteInsightsNotInstalled;
                UpdateEliteInsightsContent = Resource.InstallEliteInsights;
                installed = false;
            }
            else
            {
                EliteInsightsVersion = fileVersion;
                UpdateEliteInsightsContent = Resource.UpdateEliteInsights;
                installed = true;
            }

            ShowDetailsCommand = new RelayCommand<ParsedLogFile>(async (logFile) => await ShowDetails(logFile).ConfigureAwait(true));
            ShowProgressBar = true;

            _ = LoadDataFromDatabase();
        }

        public static string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string BaseLogResult => "LogResult";

        public static string LogResultPath => Path.Combine(AssemblyLocation, BaseLogResult);

        public BindableCollection<ParsedLogFile> LogFiles { get; private set; }

        public BindableCollection<string> BossNameFilters { get; private set; }

        public BindableCollection<string> FilesToParse { get; private set; }

        public ICommand ShowDetailsCommand { get; set; }

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

        public double Progress
        {
            get { return progress; }
            set { SetAndNotify(ref progress, value); }
        }

        public bool ShowProgressBar
        {
            get { return showProgressBar; }
            set { SetAndNotify(ref showProgressBar, value); }
        }

        public bool IsLoadingData
        {
            get { return isLoadingData; }
            set { SetAndNotify(ref isLoadingData, value); }
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

            Log.Debug("Added files for Parsing");
        }

        public async Task ParseFilesAsync()
        {
            Log.Information("{Method} called", nameof(ParseFilesAsync));

            Progress = 0;

            List<ParsedLogFile> parsedLogs = new List<ParsedLogFile>();

            await ParseLogFilesAsync(parsedLogs).ConfigureAwait(true);

            dbContext.ParsedLogFiles.AddRange(parsedLogs);

            Log.Information("Saving logs in the Database.");
            await dbContext.SaveChangesAsync().ConfigureAwait(true);
            FilesToParse.Clear();

            LogFiles.AddRange(parsedLogs);

            Progress += 20;

            bool postToDiscord = await SettingsManager.GetPostToDiscord(dbContext).ConfigureAwait(true);
            if (postToDiscord)
            {
                Log.Information("Posting the Logs to Discord.");
                await SendToDiscord(parsedLogs).ConfigureAwait(true);
            }

            Progress += 5;
        }

        public async Task UpdateEliteInsights()
        {
            await parseService.InstallEliteInsights().ConfigureAwait(true);

            string fileVersion = parseService.GetFileVersionInfo();
            EliteInsightsVersion = fileVersion;
            UpdateEliteInsightsContent = Resource.UpdateEliteInsights;

            string message = installed ? Resource.SuccessUpdate : Resource.SuccessInstalled;
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

            var logFiles = dbContext.ParsedLogFiles.AsQueryable();

            if (SelectedBossFilter.Equals(clearFilterText, StringComparison.InvariantCultureIgnoreCase))
            {
                SelectedBossFilter = null;
            }
            else
            {
                logFiles = logFiles.Where(l => l.BossName == SelectedBossFilter);
            }

            var filteredLogFiles = await logFiles.OrderBy(l => l.StartTime).ToListAsync().ConfigureAwait(true);

            LogFiles.Clear();
            LogFiles.AddRange(filteredLogFiles);
        }

        public void OpenLink(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Process.Start("explorer", path);
        }

        public async Task SendToDiscord(System.Collections.IList selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                var messageModel = new MessageViewModel(Resource.Err_SelectLogs);
                await DialogHost.Show(messageModel, DialogIdentifier).ConfigureAwait(true);

                return;
            }

            List<ParsedLogFile> selectedLogs = selectedItems.Cast<ParsedLogFile>().ToList();
            selectedLogs = selectedLogs.OrderBy(l => l.StartTime).ToList();

            await discordService.SendReports(selectedLogs).ConfigureAwait(true);
        }

        public void RemoveFile(string file)
        {
            FilesToParse.Remove(file);
        }

        public async Task ShowDetails(ParsedLogFile logFile)
        {
            var viewModel = new LogDetailsViewModel() { Test = logFile.BossName };
            await MaterialDesignThemes.Wpf.DialogHost.Show(viewModel, DialogIdentifier).ConfigureAwait(true);
        }

        public async Task DataGridKeyDown(object sender, KeyEventArgs e)
        {
            var dataGrid = (DataGrid)sender ?? throw new ArgumentNullException(nameof(sender));

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            Log.Debug("{Method} called with key {Key}", nameof(DataGridKeyDown), e.Key);

            switch (e.Key)
            {
                case Key.Delete:
                    await DeleteLogEntries(dataGrid).ConfigureAwait(true);
                    break;
                case Key.F5:
                    await RefreshGrid().ConfigureAwait(true);
                    break;
            }
        }

        public async Task DeleteLogEntries(DataGrid dataGrid)
        {
            Log.Debug("{Method} called", nameof(DeleteLogEntries));

            if (dataGrid == null)
            {
                Log.Error("No datagrid was supplied.");
                throw new ArgumentNullException(nameof(dataGrid));
            }

            var logEntries = dataGrid.SelectedItems.Cast<ParsedLogFile>();

            dbContext.ParsedLogFiles.RemoveRange(logEntries);
            await dbContext.SaveChangesAsync().ConfigureAwait(true);

            LogFiles.RemoveRange(logEntries);
        }

        public async Task RefreshGrid()
        {
            Log.Debug("{Method} called", nameof(RefreshGrid));
            await LoadDataFromDatabase().ConfigureAwait(true);
        }

        private async Task LoadDataFromDatabase()
        {
            IsLoadingData = true;

            var orderedLogs = dbContext.ParsedLogFiles.AsQueryable().OrderBy(l => l.StartTime);
            List<ParsedLogFile> parsedLogs = await orderedLogs.ToListAsync().ConfigureAwait(true);
            LogFiles.Clear();
            LogFiles.AddRange(parsedLogs);

            IsLoadingData = false;
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

        private async Task ParseLogFilesAsync(List<ParsedLogFile> parsedLogs)
        {
            const double progressIncrement = 25;
            const double parseIncrement = 50;
            bool upload = await SettingsManager.GetDpsReportUploadAsync(dbContext).ConfigureAwait(false);
            string userToken = await SettingsManager.GetUserTokenAsync(dbContext).ConfigureAwait(false);

            string htmlPath = Path.Combine(ParseService.AssemblyLocation, "Logs");

            Task<DPSReport> uploadTask = null;

            Log.Information("Parsing {Count} files", FilesToParse.Count);

            foreach (string file in FilesToParse)
            {
                if (upload)
                {
                    // upload the file...
                    uploadTask = dpsReportController.UploadToDpsReport(file, userToken);
                }

                ParsedLogFile logFile = await parseService.ParseSingleFile(file, htmlPath).ConfigureAwait(false);

                Progress += parseIncrement / FilesToParse.Count;

                parseService.ClearLogFolder();

                if (uploadTask != null)
                {
                    DPSReport report = await uploadTask.ConfigureAwait(false);

                    if (report != null)
                    {
                        logFile.DpsReportLink = report.PermaLink;
                    }
                }

                parsedLogs.Add(logFile);

                Progress += progressIncrement / FilesToParse.Count;
            }

            Log.Information("Finished parsing files.");
        }
    }
}
