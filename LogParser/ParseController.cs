using LogParser.Models.Enums;
using LogParser.Models.Interfaces;
using MaterialDesignColors.Recommended;
using RestEase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace LogParser
{
    public class ParseController
    {
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
        public static async Task<IEnumerable<string>> ParseAsync(IEnumerable<string> fileNames)
        {
            if (!IsInstalled())
            {
                throw new InvalidOperationException("There is no local installation of Elite Insights.");
            }

            if (fileNames == null || !fileNames.Any())
            {
                throw new InvalidOperationException("There are no files to parse.");
            }

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), BaseEIPath);
            string config = GetConfig(path);

            string logPath = Path.Combine(path, BaseLogPath);
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            string args = $"-p -c \"{config}\" {BuildFileArgs(fileNames)}";
            var processInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(path, EliteInsightsExecutable),
                WorkingDirectory = path,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = args,
            };

            using Process process = new Process
            {
                StartInfo = processInfo,
            };

            var task = Task.Run(() =>
            {
                process.Start();
                process.WaitForExit();
            });

            await task.ConfigureAwait(false);

            return Directory.EnumerateFiles(logPath);
        }

        public static bool IsInstalled()
        {
            string path = Path.Combine(AssemblyLocation, BaseEIPath);
            if (!Directory.Exists(path))
            {
                return false;
            }

            path = Path.Combine(path, EliteInsightsExecutable);

            return File.Exists(path);
        }

        public static FileVersionInfo GetFileVersionInfo()
        {
            if (!IsInstalled())
            {
                return null;
            }

            string path = Path.Combine(AssemblyLocation, BaseEIPath, EliteInsightsExecutable);

            return FileVersionInfo.GetVersionInfo(path);
        }

        public async static Task<Install> InstallEliteInsights()
        {
            IGitHubApi githubApi = RestClient.For<IGitHubApi>(@"https://api.github.com");
            var repo = await githubApi.GetLatestRelease("baaron4", "GW2-Elite-Insights-Parser").ConfigureAwait(true);
            if (repo == null)
            {
                return Install.Error;
            }

            FileVersionInfo fileVersion = GetFileVersionInfo();
            if (fileVersion != null && repo.Name.Contains(fileVersion.FileVersion, StringComparison.InvariantCultureIgnoreCase))
            {
                return Install.UpToDate;
            }

            var asset = repo.Assets.FirstOrDefault(a => a.Name.Equals("GW2EI.zip", StringComparison.InvariantCulture));
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

        public static void ClearLogFolder()
        {
            string logFolder = Path.Combine(AssemblyLocation, LogPath);
            if (!Directory.Exists(logFolder))
            {
                return;
            }

            Directory.Delete(logFolder, true);
        }

        private static void OnRaiseProgressChangedEvent(int progress)
        {
            ProgressChangedEvent?.Invoke(null, new ProgressChangedEventArgs(progress));
        }

        private static string GetConfig(string path)
        {
            string defaultConfig = Path.Combine(AssemblyLocation, EliteInsightsConfig);
            string destConfig = Path.Combine(path, "EIConfig.conf");
            string logPath = Path.Combine(path, BaseLogPath);

            File.Copy(defaultConfig, destConfig, true);
            File.AppendAllLines(destConfig, new string[] { @$"OutLocation={logPath}" });

            return destConfig;
        }

        private static string BuildFileArgs(IEnumerable<string> fileNames)
        {
            string args = string.Empty;

            foreach (string file in fileNames)
            {
                args += $"\"{file}\" ";
            }

            return args;
        }
    }
}
