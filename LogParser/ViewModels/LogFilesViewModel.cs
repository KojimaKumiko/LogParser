using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using LogParser.Events;
using LogParser.Models;
using LogParser.Services;
using Serilog;
using Stylet;

namespace LogParser.ViewModels
{
    public class LogFilesViewModel : Screen, IHandle<FilesAddedEvent>, IHandle<StartParsingEvent>
    {
        private const double progressIncrement = 25;

        private BindableCollection<string> filesToParse;

        private readonly IEventAggregator eventAggregator;
        private readonly IParseService parseService;
        private readonly DatabaseContext dbContext;
        private readonly DpsReportService dpsReportService;

        public LogFilesViewModel(
            IEventAggregator eventAggregator,
            IParseService parseService,
            DatabaseContext dbContext,
            DpsReportService dpsReportService)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.parseService = parseService ?? throw new ArgumentNullException(nameof(parseService));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.dpsReportService =
                dpsReportService ?? throw new ArgumentNullException(nameof(dpsReportService));
            
            this.eventAggregator.Subscribe(this);
            
            DisplayName = "Log Files";
            FilesToParse = new BindableCollection<string>();
            FilesToParse.CollectionChanged += (sender, args) => NotifyOfPropertyChange(nameof(CanParseFiles));
        }

        public BindableCollection<string> FilesToParse
        {
            get => filesToParse;
            set => SetAndNotify(ref filesToParse, value);
        }

        public object CanParseFiles => FilesToParse != null && FilesToParse.Count > 0;

        public void RemoveFile(string file)
        {
            FilesToParse.Remove(file);
        }

        public void Handle(FilesAddedEvent message)
        {
            if (message == null)
            {
                return;
            }
            
            FilesToParse.AddRange(message.Files);
        }

        public async void Handle(StartParsingEvent message)
        {
            if (message == null)
            {
                return;
            }
            
            var parsedLogs = new List<ParsedLogFileDto>();
            var upload = await SettingsManager.GetDpsReportUploadAsync(dbContext).ConfigureAwait(false);
            var userToken = await SettingsManager.GetUserTokenAsync(dbContext).ConfigureAwait(false);

            var htmlPath = Path.Combine(ParseService.AssemblyLocation, "Logs");

            string eiPath = Path.Combine(ParseService.AssemblyLocation, ParseService.BaseEIPath);

            string logPath = Path.Combine(eiPath, ParseService.BaseLogPath);
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            using var watcher = new FileSystemWatcher(logPath);
            watcher.Created += OnCreated;
            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.Filter = "*.html";
            watcher.EnableRaisingEvents = true;

            var parsedFiles = await parseService.ParseFiles(FilesToParse, eiPath, logPath);

            watcher.EnableRaisingEvents = false;

            Task<DPSReport> uploadTask = null;

            Log.Information("Parsing {Count} files", FilesToParse.Count);

            foreach (var file in FilesToParse)
            {
                if (upload)
                {
                    // upload the file...
                    uploadTask = dpsReportService.UploadToDpsReport(file, userToken);
                }

                //var logFile = await parseService.ParseSingleFile(file, htmlPath).ConfigureAwait(false);
                var fileName = Path.GetFileNameWithoutExtension(file);
                var logFile = parseService.ParseSingleFile(parsedFiles.Where(p => p.Contains(fileName)), htmlPath);

                PublishProgressIncrement(progressIncrement / FilesToParse.Count);

                if (uploadTask != null)
                {
                    var report = await uploadTask.ConfigureAwait(false);

                    if (report != null)
                    {
                        logFile.DpsReportLink = report.PermaLink;
                    }
                }

                parsedLogs.Add(logFile);
                PublishProgressIncrement(progressIncrement / FilesToParse.Count);
            }

            parseService.ClearLogFolder();

            eventAggregator.Publish(new ParsingFinishedEvent(parsedLogs));
            FilesToParse.Clear();
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            PublishProgressIncrement(progressIncrement / FilesToParse.Count);
        }

        private void PublishProgressIncrement(double progressIncrement)
        {
            eventAggregator.Publish(new ProgressEvent(progressIncrement));
        }
    }
}
