using Database;
using Database.Models;
using LogParser.Models;
using LogParser.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        /// <summary>
        /// Maps the name of the class to a dictionary, which maps the name of the properties of the json to the properties of the class.
        /// </summary>
        private Dictionary<string, Dictionary<string, PropertyInfo>> JsonProperties { get; set; }

        public LogParserViewModel()
        {
            LogFiles = new BindableCollection<ParsedLogFile>();
            FilesToParse = new BindableCollection<string>();
            BossNameFilters = new BindableCollection<string>()
            {
                clearFilterText
            };
            JsonProperties = new Dictionary<string, Dictionary<string, PropertyInfo>>();

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

            _ = LoadDataFromDatabase();
        }

        public static string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string BaseLogResult => "LogResult";

        public static string LogResultPath => Path.Combine(AssemblyLocation, BaseLogResult);

        public BindableCollection<ParsedLogFile> LogFiles { get; private set; }

        public BindableCollection<string> BossNameFilters { get; private set; }

        public BindableCollection<string> FilesToParse { get; private set; }

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
            List<string> parsedFiles = (await ParseController.ParseAsync(FilesToParse).ConfigureAwait(false)).ToList();

            if (parsedFiles == null || parsedFiles.Count <= 0)
            {
                return;
            }

            List<ParsedLogFile> parsedLogs = new List<ParsedLogFile>();

            foreach (string file in parsedFiles.Where(p => p.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase)))
            {
                using StreamReader sr = File.OpenText(file);
                using JsonReader reader = new JsonTextReader(sr);

                string currentProp = string.Empty;
                string[] propsToSkip = new string[]
                {
                    "targets",
                    //"players",
                    "phases",
                    "mechanics",
                    "uploadLinks",
                    "skillMap",
                    "buffMap",
                    "damageModMap",
                    "personalBuffs",
                    "weapons",
                    "targetDamage1S",
                    "targetDamageDist",
                    "statsTargets",
                    "support",
                    "damageModifiers",
                    "damageModifiersTarget",
                    "buffUptimes",
                    "selfBuffs",
                    "groupBuffs",
                    "offGroupBuffs",
                    "squadBuffs",
                    "buffUptimesActive",
                    "selfBuffsActive",
                    "groupBuffsActive",
                    "offGroupBuffsActive",
                    "squadBuffsActive",
                    "consumables",
                    "activeTimes",
                    "minions",
                    "dpsAll",
                    "statsAll",
                    "defenses",
                    "totalDamageDist",
                    "totalDamageTaken",
                    "rotation",
                    "actorPowerDamage",
                };

                ParsedLogFile logFile = new ParsedLogFile();
                logFile.Players = new List<LogPlayer>();

                List<LogPlayer> logPlayers = new List<LogPlayer>();
                LogPlayer logPlayer = new LogPlayer();

                DpsTarget dpsTarget = new DpsTarget();

                bool parseLogFile = true;
                bool parsePlayers = false;
                bool parseDpsTargets = false;

                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        if (reader.TokenType == JsonToken.PropertyName)
                        {
                            currentProp = reader.Value.ToString();

                            if (currentProp.Equals("players", StringComparison.InvariantCultureIgnoreCase))
                            {
                                parsePlayers = true;
                            }
                            else if (currentProp.Equals("dpsTargets", StringComparison.InvariantCultureIgnoreCase))
                            {
                                parseDpsTargets = true;
                            }
                        }

                        if (reader.TokenType != JsonToken.PropertyName)
                        {
                            if (parseLogFile)
                            {
                                PopulateLogFile(logFile, currentProp, reader.Value);
                            }
                            else if (parsePlayers)
                            {
                                if (parseDpsTargets)
                                {
                                    PopulateLogFile(dpsTarget, currentProp, reader.Value);
                                }
                                else
                                {
                                    PopulateLogFile(logPlayer, currentProp, reader.Value);
                                }
                            }
                        }
                    }
                    else if ((reader.TokenType == JsonToken.StartArray || reader.TokenType == JsonToken.StartObject) && propsToSkip.Contains(currentProp))
                    {
                        reader.Skip();
                        parseLogFile = false;

                        if (currentProp.Equals("rotation", StringComparison.InvariantCultureIgnoreCase))
                        {
                            logPlayer.DpsTargets = new List<DpsTarget> { dpsTarget };
                            dpsTarget = new DpsTarget();

                            logPlayers.Add(logPlayer);
                            logPlayer = new LogPlayer();

                            currentProp = string.Empty;
                        }
                    }
                    else if (reader.TokenType == JsonToken.EndObject && currentProp.Equals("actorPowerDamage", StringComparison.InvariantCultureIgnoreCase))
                    {
                        parseDpsTargets = false;
                    }
                }

                logFile.Players.AddRange(logPlayers);

                parsedLogs.Add(logFile);
            }

            using DatabaseContext db = new DatabaseContext();

            bool uploadToDpsReport = await SettingsManager.GetDpsReportUploadAsync(db).ConfigureAwait(true);

            if (uploadToDpsReport)
            {
                IDpsReport dpsReportApi = RestClient.For<IDpsReport>(@"https://dps.report");

                foreach (string file in FilesToParse)
                {
                    string fileName = file.Split("\\").Last();
                    byte[] fileContent = await File.ReadAllBytesAsync(file).ConfigureAwait(true);

                    using MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----myBoundary");
                    using ByteArrayContent byteArrayContent = new ByteArrayContent(fileContent);

                    byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
                    multiPartContent.Add(byteArrayContent, "file", fileName);

                    try
                    {
                        var response = dpsReportApi.UploadContent(multiPartContent);
                        Debug.WriteLine(response);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }

            db.ParsedLogFiles.AddRange(parsedLogs);

            await db.SaveChangesAsync().ConfigureAwait(false);

            //ParseController.ClearLogFolder();
            FilesToParse.Clear();

            // load the new logs in
            await LoadDataFromDatabase().ConfigureAwait(true);
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

        public async void FormData()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = ".html|*.html;*.htm"
            };
            bool result = (bool)fileDialog.ShowDialog();

            if (result)
            {
                string fileName = fileDialog.FileName;

                IDpsReport testApi = RestClient.For<IDpsReport>(@"https://b.dps.report");
                byte[] fileContent = await File.ReadAllBytesAsync(fileName).ConfigureAwait(true);
                
                using var multiPartContent = new MultipartFormDataContent("----myBoundary");
                using var byteArrayContent = new ByteArrayContent(fileContent);

                byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
                multiPartContent.Add(byteArrayContent, "file", fileName);

                try
                {
                    var test = testApi.UploadContent(multiPartContent);
                    Debug.WriteLine(test);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void PopulateLogFile<T>(T logObject, string propertyName, object value)
        {
            var logType = logObject.GetType();
            if (JsonProperties.Count <= 0 || !JsonProperties.ContainsKey(logType.Name))
            {
                var dictionary = new Dictionary<string, PropertyInfo>();
                var properties = logType.GetProperties();

                foreach (var prop in properties)
                {
                    JsonPropertyAttribute jsonPropertyAttribute = (JsonPropertyAttribute)Attribute.GetCustomAttribute(prop, typeof(JsonPropertyAttribute));
                    if (jsonPropertyAttribute != null)
                    {
                        dictionary.Add(jsonPropertyAttribute.PropertyName, prop);
                    }
                }

                JsonProperties.Add(logType.Name, dictionary);
            }

            JsonProperties.TryGetValue(logType.Name, out var jsonDictionary);

            if (jsonDictionary == null)
            {
                return;
            }

            jsonDictionary.TryGetValue(propertyName, out var propInfo);
            if (propInfo == null)
            {
                return;
            }

            if (propInfo.PropertyType == typeof(DateTime))
            {
                value = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
            }

            propInfo.SetValue(logObject, value);
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
