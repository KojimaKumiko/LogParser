using Database;
using Database.Models;
using LogParser.Services;
using LogParser.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Serilog;
using Stylet;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using LogParser.Events;
using MaterialDesignThemes.Wpf;
using LogParser.Models;
using AutoMapper;
using System.Collections.Generic;

namespace LogParser.ViewModels
{
    public class LogParserViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<ParsingFinishedEvent>, IHandle<ProgressEvent>
    {
        private const string DialogIdentifier = "RootDialogHost";
        private const string ClearFilterText = " --- NONE --- ";

        private readonly IMapper mapper;
        private readonly DatabaseContext dbContext;
        private readonly IParseService parseService;
        private readonly IDiscordService discordService;
        private readonly SnackbarMessageQueue messageQueue;
        private readonly IEventAggregator eventAggregator;

        private string selectedBossFilter;
        private string eliteInsightsVersion;
        private string updateEliteInsightsContent;
        private double progress;
        private bool showProgressBar;
        private bool installed;
        private bool isLoadingData;

        public LogParserViewModel(
            IMapper mapper,
            DatabaseContext dbContext,
            IParseService parseService,
            IDiscordService discordService,
            SnackbarMessageQueue messageQueue,
            IEventAggregator eventAggregator,
            LogFilesViewModel logFilesViewModel,
            LogDetailsViewModel logDetailsViewModel)
        {
            Log.Debug("LogParserViewModel constructor called");

            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.parseService = parseService ?? throw new ArgumentNullException(nameof(parseService));
            this.discordService = discordService ?? throw new ArgumentNullException(nameof(discordService));
            this.messageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            if (logFilesViewModel == null)
            {
                throw new ArgumentNullException(nameof(logFilesViewModel));
            }

            if (logDetailsViewModel == null)
            {
                throw new ArgumentNullException(nameof(logDetailsViewModel));
            }

            this.eventAggregator.Subscribe(this);

            Items.Add(logFilesViewModel);
            //Items.Add(logDetailsViewModel);

            LogFiles = new BindableCollection<ParsedLogFileDto>();
            BossNameFilters = new BindableCollection<string>()
            {
                ClearFilterText
            };

            LogFiles.CollectionChanged += LogFilesCollectionChanged;

            var fileVersion = parseService.GetFileVersionInfo();
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
            
            ShowProgressBar = true;

            _ = LoadDataFromDatabase();
        }

        private static string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string BaseLogResult => "LogResult";
        public static string LogResultPath => Path.Combine(AssemblyLocation, BaseLogResult);
        public BindableCollection<ParsedLogFileDto> LogFiles { get; private set; }
        public BindableCollection<string> BossNameFilters { get; private set; }
        public ICommand ShowDetailsCommand { get; set; }

        public string SelectedBossFilter
        {
            get => selectedBossFilter;
            set => SetAndNotify(ref selectedBossFilter, value);
        }
        public string EliteInsightsVersion
        {
            get => eliteInsightsVersion;
            set => SetAndNotify(ref eliteInsightsVersion, value);
        }
        public string UpdateEliteInsightsContent
        {
            get => updateEliteInsightsContent;
            private set => SetAndNotify(ref updateEliteInsightsContent, value);
        }
        public double Progress
        {
            get => progress;
            set => SetAndNotify(ref progress, value);
        }
        public bool ShowProgressBar
        {
            get => showProgressBar;
            set => SetAndNotify(ref showProgressBar, value);
        }
        public bool IsLoadingData
        {
            get => isLoadingData;
            set => SetAndNotify(ref isLoadingData, value);
        }

        public void SetFile()
        {
            var fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = ".evtc, .zevtc|*.evtc;*.zevtc"
            };
            var result = fileDialog.ShowDialog();

            if (result != null && result.Value)
            {
                eventAggregator.Publish(new FilesAddedEvent(fileDialog.FileNames));
            }
        }

        public void ParseFilesAsync()
        {
            Log.Information("{Method} called", nameof(ParseFilesAsync));

            Progress = 0;

            eventAggregator.Publish(new StartParsingEvent());
        }

        public async Task UpdateEliteInsights()
        {
            await parseService.InstallEliteInsights().ConfigureAwait(true);

            var fileVersion = parseService.GetFileVersionInfo();
            EliteInsightsVersion = fileVersion;
            UpdateEliteInsightsContent = Resource.UpdateEliteInsights;

            var message = installed ? Resource.SuccessUpdate : Resource.SuccessInstalled;
            var messageModel = new MessageDialog(message);
            await DialogHost.Show(messageModel, DialogIdentifier).ConfigureAwait(true);

            installed = true;
        }

        public async void BossFilterChanged(object sender, SelectionChangedEventArgs eventArgs)
        {
            if (string.IsNullOrWhiteSpace(selectedBossFilter) || eventArgs == null)
            {
                return;
            }

            var logFiles = dbContext.ParsedLogFiles.AsQueryable().AsNoTracking();

            if (SelectedBossFilter.Equals(ClearFilterText, StringComparison.InvariantCultureIgnoreCase))
            {
                SelectedBossFilter = null;
            }
            else
            {
                logFiles = logFiles.Where(l => l.BossName == SelectedBossFilter);
            }

            var filteredLogFiles = await logFiles.OrderBy(l => l.StartTime).ToListAsync().ConfigureAwait(true);
            var logFileDtos = mapper.Map<List<ParsedLogFileDto>>(filteredLogFiles);

            LogFiles.Clear();
            LogFiles.AddRange(logFileDtos);
        }

        public void OpenLink(string path)
        {
            Helper.OpenLink(path);
        }

        public async Task SendToDiscord(System.Collections.IList selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                var messageModel = new MessageDialog(Resource.Err_SelectLogs);
                await DialogHost.Show(messageModel, DialogIdentifier).ConfigureAwait(true);

                return;
            }

            var selectedLogs = selectedItems.Cast<ParsedLogFileDto>().ToList();
            selectedLogs = selectedLogs.OrderBy(l => l.StartTime).ToList();

            await discordService.SendReports(selectedLogs).ConfigureAwait(true);
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
                Log.Error("No datagrid was supplied");
                throw new ArgumentNullException(nameof(dataGrid));
            }

            var logEntries = dataGrid.SelectedItems.Cast<ParsedLogFileDto>();

            var parsedLogFiles = logEntries.ToList();

            var logFiles = dbContext.ParsedLogFiles.AsQueryable().Where(log => parsedLogFiles.Select(l => l.ID).Contains(log.ID)).ToList();
            dbContext.RemoveRange(logFiles);

            await dbContext.SaveChangesAsync().ConfigureAwait(true);

            LogFiles.RemoveRange(parsedLogFiles);
        }

        public async Task RefreshGrid()
        {
            Log.Debug("{Method} called", nameof(RefreshGrid));
            await LoadDataFromDatabase().ConfigureAwait(true);
        }
        
        public async void Handle(ParsingFinishedEvent message)
        {
            if (message == null)
            {
                return;
            }
            
            Log.Information("Finished parsing files");

            var parsedLogs = message.ParsedLogFiles;
            var logFiles = mapper.Map<List<ParsedLogFile>>(parsedLogs);
            await dbContext.ParsedLogFiles.AddRangeAsync(logFiles);

            Log.Information("Saving logs in the Database");
            await dbContext.SaveChangesAsync().ConfigureAwait(true);

            LogFiles.AddRange(parsedLogs);
            
            Progress += 20;
            
            var postToDiscord = await SettingsManager.GetPostToDiscord(dbContext).ConfigureAwait(true);
            if (postToDiscord)
            {
                Log.Information("Posting the Logs to Discord");
                await SendToDiscord(parsedLogs).ConfigureAwait(true);
            }
            
            Progress += 5;
        }

        public void Handle(ProgressEvent message)
        {
            if (message == null)
            {
                return;
            }

            Progress += message.ProgressIncrement;
        }

        private async Task LoadDataFromDatabase()
        {
            IsLoadingData = true;

            var orderedLogs = dbContext.ParsedLogFiles.AsQueryable().AsNoTracking().OrderBy(l => l.StartTime);
            var parsedLogs = await orderedLogs.ToListAsync().ConfigureAwait(true);
            var logFiles = mapper.Map<List<ParsedLogFileDto>>(parsedLogs);

            LogFiles.Clear();
            LogFiles.AddRange(logFiles);

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
    }
}
