using Database.Models;
using LogParser.Models.Enums;
using LogParser.Models.Interfaces;
using Newtonsoft.Json;
using RestEase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace LogParser.Controller
{
    public class ParseController
    {
        private static Dictionary<string, Dictionary<string, PropertyInfo>> JsonProperties { get; }
            = new Dictionary<string, Dictionary<string, PropertyInfo>>();

        public ParseController()
        {
        }

        public static string BaseEIPath => @"EliteInsights";
        public static string BaseLogPath => @"Logs";
        public static string EliteInsightsExecutable => "GuildWars2EliteInsights.exe";
        public static string EliteInsightsConfig => "EliteInsightsConfig.conf";
        public static string LogPath => Path.Combine(BaseEIPath, BaseLogPath);
        public static string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static event EventHandler<ProgressChangedEventArgs> ProgressChangedEvent;

        /// <summary>
        /// Parses the files with the current installed version of Elite Insights.
        /// </summary>
        /// <param name="fileNames">The files to parse. The full path is required.</param>
        /// <returns>A list of strings containing the paths for the generated .json and .html files for each parsed file.</returns>
        public async Task<ParsedLogFile> ParseSingleFile(string fileName, string htmlPath)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!IsInstalled())
            {
                throw new InvalidOperationException("There is no local installation of Elite Insights.");
            }

            string path = Path.Combine(AssemblyLocation, BaseEIPath);

            string logPath = Path.Combine(path, BaseLogPath);
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            IEnumerable<string> parsedFiles = await ParseFile(fileName, path, logPath).ConfigureAwait(false);

            string jsonFile = parsedFiles.First(f => f.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase));
            using StreamReader sr = File.OpenText(jsonFile);
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

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.Value != null)
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        currentProp = reader.Value.ToString();

                        if (currentProp.Equals("players", StringComparison.OrdinalIgnoreCase))
                        {
                            parsePlayers = true;
                        }
                        else if (currentProp.Equals("dpsTargets", StringComparison.OrdinalIgnoreCase))
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
                    Task skipTask = reader.SkipAsync();
                    parseLogFile = false;

                    if (currentProp.Equals("rotation", StringComparison.OrdinalIgnoreCase))
                    {
                        logPlayer.DpsTargets = new List<DpsTarget> { dpsTarget };
                        dpsTarget = new DpsTarget();

                        logPlayers.Add(logPlayer);
                        logPlayer = new LogPlayer();

                        currentProp = string.Empty;
                    }

                    await skipTask.ConfigureAwait(false);
                }
                else if (reader.TokenType == JsonToken.EndObject && currentProp.Equals("actorPowerDamage", StringComparison.OrdinalIgnoreCase))
                {
                    parseDpsTargets = false;
                }
            }

            if (!Directory.Exists(htmlPath))
            {
                Directory.CreateDirectory(htmlPath);
            }

            string htmlFile = parsedFiles.First(f => f.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase));
            string htmlFileName = Path.GetFileName(htmlFile);
            htmlPath = Path.Combine(htmlPath, htmlFileName);
            File.Move(htmlFile, htmlPath, true);
            logFile.HtmlPath = htmlPath;

            logFile.Players.AddRange(logPlayers);

            return logFile;
        }

        public bool IsInstalled()
        {
            string path = Path.Combine(AssemblyLocation, BaseEIPath);
            if (!Directory.Exists(path))
            {
                return false;
            }

            path = Path.Combine(path, EliteInsightsExecutable);

            return File.Exists(path);
        }

        public FileVersionInfo GetFileVersionInfo()
        {
            if (!IsInstalled())
            {
                return null;
            }

            string path = Path.Combine(AssemblyLocation, BaseEIPath, EliteInsightsExecutable);

            return FileVersionInfo.GetVersionInfo(path);
        }

        public async Task<Install> InstallEliteInsights()
        {
            IGitHubApi githubApi = RestClient.For<IGitHubApi>(new Uri(@"https://api.github.com"));
            var repo = await githubApi.GetLatestRelease("baaron4", "GW2-Elite-Insights-Parser").ConfigureAwait(false);
            if (repo == null)
            {
                return Install.Error;
            }

            FileVersionInfo fileVersion = GetFileVersionInfo();
            if (fileVersion != null && repo.Name.Contains(fileVersion.FileVersion, StringComparison.InvariantCultureIgnoreCase))
            {
                return Install.UpToDate;
            }

            var asset = repo.Assets.FirstOrDefault(a => a.Name.Equals("GW2EI.zip", StringComparison.Ordinal));
            if (asset == null)
            {
                return Install.Error;
            }

            string destinationPath = Path.Combine(AssemblyLocation, BaseEIPath);
            if (Directory.Exists(destinationPath))
            {
                Directory.Delete(destinationPath, true);
            }

            Directory.CreateDirectory(destinationPath);

            string zipFile = Path.Combine(destinationPath, asset.Name);
            using var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "LogParser");
            client.DownloadFile(asset.DownloadUrl, zipFile);

            if (File.Exists(zipFile))
            {
                ZipFile.ExtractToDirectory(zipFile, destinationPath);
                File.Delete(zipFile);
            }

            return Install.Success;
        }

        public void ClearLogFolder()
        {
            string logFolder = Path.Combine(AssemblyLocation, LogPath);
            if (!Directory.Exists(logFolder))
            {
                return;
            }

            Directory.Delete(logFolder, true);
        }

        private void OnRaiseProgressChangedEvent(int progress)
        {
            ProgressChangedEvent?.Invoke(null, new ProgressChangedEventArgs(progress));
        }

        private string GetConfig(string path)
        {
            string defaultConfig = Path.Combine(AssemblyLocation, EliteInsightsConfig);
            string destConfig = Path.Combine(path, "EIConfig.conf");
            string logPath = Path.Combine(path, BaseLogPath);

            File.Copy(defaultConfig, destConfig, true);
            File.AppendAllLines(destConfig, new string[] { @$"OutLocation={logPath}" });

            return destConfig;
        }

        private async Task<IEnumerable<string>> ParseFile(string fileName, string eiPath, string logPath)
        {
            string config = GetConfig(eiPath);
            string args = $"-p -c \"{config}\" \"{fileName}\"";
            var processInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(eiPath, EliteInsightsExecutable),
                WorkingDirectory = eiPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = args,
            };

            using Process process = new Process
            {
                StartInfo = processInfo,
            };

            // Start the process and wait for it until it's finished.
            Task processTask = Task.Run(() =>
            {
                process.Start();
                process.WaitForExit();
                process.Close();
            });

            await processTask.ConfigureAwait(false);

            return Directory.EnumerateFiles(logPath);
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
    }
}
