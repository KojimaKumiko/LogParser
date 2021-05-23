using Database.Models;
using LogParser.Models;
using LogParser.Models.Enums;
using LogParser.Models.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

namespace LogParser.Services
{
    public class ParseService : IParseService
    {
        public ParseService()
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
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fileNames"><inheritdoc/></param>
        /// <param name="htmlPath"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public ParsedLogFileDto ParseSingleFile(IEnumerable<string> fileNames, string htmlPath)
        {
            if (fileNames == null || !fileNames.Any())
            {
                throw new ArgumentNullException(nameof(fileNames));
            }

            if (!IsInstalled())
            {
                throw new InvalidOperationException("There is no local installation of Elite Insights.");
            }

            string jsonFile = fileNames.First(f => f.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase));

            using var reader = File.OpenText(jsonFile);
            using var jsonReader = new JsonTextReader(reader);
            JObject jsonData = (JObject)JToken.ReadFrom(jsonReader);

            var logPlayers = ((JArray)jsonData["players"]).Select(player =>
            {
                JObject dpsTargetJson = (JObject)player["dpsTargets"].First.First;
                var dpsTarget = new DpsTargetDto(dpsTargetJson);

                return new LogPlayerDto((JObject)player, new List<DpsTargetDto>() { dpsTarget });
            }).ToList();

            var parsedLogFile = new ParsedLogFileDto(jsonData, logPlayers);

            if (!Directory.Exists(htmlPath))
            {
                Directory.CreateDirectory(htmlPath);
            }

            string htmlFile = fileNames.First(f => f.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase));
            string htmlFileName = Path.GetFileName(htmlFile);
            htmlPath = Path.Combine(htmlPath, htmlFileName);
            File.Move(htmlFile, htmlPath, true);
            parsedLogFile.HtmlPath = htmlPath;

            return parsedLogFile;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="fileName"> <inheritdoc/> </param>
        /// <param name="eiPath"> <inheritdoc/> </param>
        /// <param name="logPath"> <inheritdoc/> </param>
        /// <returns> <inheritdoc/> </returns>
        public async Task<IEnumerable<string>> ParseFiles(IEnumerable<string> fileNames, string eiPath, string logPath)
        {
            string config = GetConfig(eiPath);
            string args = $"-p -c \"{config}\" \"{string.Join("\" \"", fileNames)}\"";
            var processInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(eiPath, EliteInsightsExecutable),
                WorkingDirectory = eiPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = args,
            };

            using Process process = new()
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public string GetFileVersionInfo()
        {
            if (!IsInstalled())
            {
                return null;
            }

            string path = Path.Combine(AssemblyLocation, BaseEIPath, EliteInsightsExecutable);

            return FileVersionInfo.GetVersionInfo(path).FileVersion;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public async Task<Install> InstallEliteInsights()
        {
            IGitHubApi githubApi = RestClient.For<IGitHubApi>(new Uri(@"https://api.github.com"));
            var repo = await githubApi.GetLatestRelease("baaron4", "GW2-Elite-Insights-Parser").ConfigureAwait(false);
            if (repo == null)
            {
                return Install.Error;
            }

            string fileVersion = GetFileVersionInfo();
            if (!string.IsNullOrWhiteSpace(fileVersion) && repo.TagName.Contains(fileVersion, StringComparison.InvariantCultureIgnoreCase))
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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
    }
}
