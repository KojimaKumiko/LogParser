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
            //OpenFileDialog fileDialog = new OpenFileDialog
            //{
            //    Multiselect = false
            //};
            //bool result = (bool)fileDialog.ShowDialog();

            //if (result)
            //{
            //    using var fs = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            //    if (fileDialog.FileName.EndsWith(".zevtc", StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        using var arch = new ZipArchive(fs, ZipArchiveMode.Read);

            //        if (arch.Entries.Count != 1)
            //        {
            //            Debug.WriteLine("Something is fishy");
            //        }

            //        using var data = arch.Entries[0].Open();
            //        using var ms = new MemoryStream();

            //        data.CopyTo(ms);
            //        ms.Position = 0;
            //        ParseData(ms);
            //    }
            //    else
            //    {
            //        ParseData(fs);
            //    }
            //}

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

                LogFiles.Add(logFile);
            }

            using DatabaseContext db = new DatabaseContext();

            db.ParsedLogFiles.AddRange(LogFiles);

            await db.SaveChangesAsync().ConfigureAwait(false);

            ParseController.ClearLogFolder();
            FilesToParse.Clear();
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

        public async Task Professions()
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/content/Specs.json";
            if (!File.Exists(filePath))
            {
                var fs = File.Create(filePath);
                fs.Close();
            }

            IGuildWars2Api guildWars2Api = RestClient.For<IGuildWars2Api>("https://api.guildwars2.com/v2");

            List<Specialization> specsToAdd = new List<Specialization>();
            List<int> specIds = await guildWars2Api.GetSpecializationsAsync().ConfigureAwait(true);
            Debug.WriteLine($"Got {specIds.Count} IDs");
            foreach (var id in specIds)
            {
                Specialization spec = await guildWars2Api.GetSpecializationAsync(id).ConfigureAwait(true);
                if (spec != null)
                {
                    Debug.WriteLine($"Adding {spec.Name} to the list.");
                    specsToAdd.Add(spec);
                }
            }

            using var writer = new StreamWriter(filePath);
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            };
            serializer.Serialize(writer, specsToAdd);
        }

        public void ReadFromDatabase()
        {
            using DatabaseContext db = new DatabaseContext();
            LogFiles.AddRange(db.ParsedLogFiles);
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

        public async void GetGithubRepo()
        {
            IGitHubApi githubApi = RestClient.For<IGitHubApi>(@"https://api.github.com");
            var repo = await githubApi.GetLatestRelease("baaron4", "GW2-Elite-Insights-Parser").ConfigureAwait(true);
            Debug.WriteLine(repo);
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

        //private void ParseData(Stream stream)
        //{
        //    using var reader = new BinaryReader(stream, new System.Text.UTF8Encoding());
        //    string buildVersion = GetString(stream, 12);

        //    Debug.WriteLine($"Build Version: {buildVersion}");

        //    byte revision = reader.ReadByte();
        //    Debug.WriteLine($"Revision: {revision}");

        //    var id = reader.ReadUInt16();
        //    Debug.WriteLine($"ID: {id}");

        //    SafeSkip(stream, 1);

        //    // Agent Data
        //    // 4 bytes: player count
        //    int agentCount = reader.ReadInt32();
        //    List<AgentData> agents = new List<AgentData>();

        //    for (int i = 0; i < agentCount; i++)
        //    {
        //        AgentData agent = new AgentData
        //        {
        //            Agent = reader.ReadUInt64(),
        //            Prof = reader.ReadUInt32(),
        //            IsElite = reader.ReadUInt32(),
        //            Toughness = reader.ReadUInt16(),
        //            Concentration = reader.ReadUInt16(),
        //            Healing = reader.ReadUInt16(),
        //            HitboxWidth = reader.ReadUInt16(),
        //            Condition = reader.ReadUInt16(),
        //            HitboxHeight = reader.ReadUInt16(),
        //            Name = GetString(stream, 68, false),
        //        };

        //        if (agent.IsElite == 0xFFFFFFFF)
        //        {
        //            if ((agent.Prof & 0xffff0000) == 0xffff0000)
        //            {
        //                Debug.WriteLine("Gadget");
        //            }
        //            else
        //            {
        //                Debug.WriteLine("NPC");
        //            }
        //        }

        //        agents.Add(agent);
        //    }

        //    // Skill Data
        //    // 4 bytes: player count
        //    uint skillCount = reader.ReadUInt32();

        //    List<SkillData> skills = new List<SkillData>();
        //    // 68 bytes: each skill
        //    for (int i = 0; i < skillCount; i++)
        //    {
        //        SkillData skill = new SkillData
        //        {
        //            SkillID = reader.ReadInt32(), // 4 bytes: Skill ID
        //            Name = GetString(stream, 64), // 64 bytes: Skill Name
        //        };

        //        skills.Add(skill);
        //    }

        //    long cbtItemCount = (reader.BaseStream.Length - reader.BaseStream.Position) / 64;
        //    List<CombatData> combats = new List<CombatData>();
        //    for (long i = 0; i < cbtItemCount; i++)
        //    {
        //        CombatData combatData = new CombatData
        //        {
        //            Time = reader.ReadUInt64(),
        //            SrcAgent = reader.ReadUInt64(),
        //            DstAgent = reader.ReadUInt64(),
        //            Value = reader.ReadInt32(),
        //            BuffDmg = reader.ReadInt32(),
        //            OverstackValue = reader.ReadUInt32(),
        //            SkillID = reader.ReadUInt32(),
        //            SrcInstID = reader.ReadUInt16(),
        //            DstInstID = reader.ReadUInt16(),
        //            SrcMasterInstID = reader.ReadUInt16(),
        //            DstMasterInstID = reader.ReadUInt16(),
        //            IFF = reader.ReadByte(),
        //            Buff = reader.ReadByte(),
        //            Result = reader.ReadByte(),
        //            IsActivation = reader.ReadByte(),
        //            IsBuffRemove = reader.ReadByte(),
        //            IsNinety = reader.ReadByte(),
        //            IsFifty = reader.ReadByte(),
        //            IsMoving = reader.ReadByte(),
        //            IsStateChange = reader.ReadByte(),
        //            IsFlanking = reader.ReadByte(),
        //            IsShields = reader.ReadByte(),
        //            IsOffcycle = reader.ReadByte(),
        //            Pad = reader.ReadUInt32(),
        //        };

        //        combats.Add(combatData);
        //    }
        //}

        //private string GetString(Stream stream, int length, bool nullTerminated = true)
        //{
        //    byte[] bytes = new byte[length];
        //    stream.Read(bytes, 0, length);
        //    if (nullTerminated)
        //    {
        //        for (int i = 0; i < length; ++i)
        //        {
        //            if (bytes[i] == 0)
        //            {
        //                length = i;
        //                break;
        //            }
        //        }
        //    }
        //    return System.Text.Encoding.UTF8.GetString(bytes, 0, length);
        //}

        //private void SafeSkip(Stream stream, long bytesToSkip)
        //{
        //    if (stream.CanSeek)
        //    {
        //        stream.Seek(bytesToSkip, SeekOrigin.Current);
        //    }
        //    else
        //    {
        //        while (bytesToSkip > 0)
        //        {
        //            stream.ReadByte();
        //            --bytesToSkip;
        //        }
        //    }
        //}
    }
}
