using System;
using System.Collections.Generic;
using System.IO;
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
            
            const double progressIncrement = 25;
            const double parseIncrement = 50;
            var parsedLogs = new List<ParsedLogFile>();
            var upload = await SettingsManager.GetDpsReportUploadAsync(dbContext).ConfigureAwait(false);
            var userToken = await SettingsManager.GetUserTokenAsync(dbContext).ConfigureAwait(false);

            var htmlPath = Path.Combine(ParseService.AssemblyLocation, "Logs");

            Task<DPSReport> uploadTask = null;

            Log.Information("Parsing {Count} files", FilesToParse.Count);

            foreach (var file in FilesToParse)
            {
                if (upload)
                {
                    // upload the file...
                    uploadTask = dpsReportService.UploadToDpsReport(file, userToken);
                }

                var logFile = await parseService.ParseSingleFile(file, htmlPath).ConfigureAwait(false);

                PublishProgressIncrement(parseIncrement / FilesToParse.Count);

                parseService.ClearLogFolder();

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
            
            eventAggregator.Publish(new ParsingFinishedEvent(parsedLogs));
            FilesToParse.Clear();
        }

        private void PublishProgressIncrement(double progressIncrement)
        {
            eventAggregator.Publish(new ProgressEvent(progressIncrement));
        }
    }
}
