using MaterialDesignColors.Recommended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LogParser
{
    public class ParseController
    {
        public static string BaseEIPath => @"EliteInsights";
        public static string BaseLogPath => @"Logs";
        public static string EliteInsightsExecutable => "GuildWars2EliteInsights.exe";
        public static string EliteInsightsConfig => "EliteInsightsConfig.conf";
        public static string LogPath => Path.Combine(BaseEIPath, BaseLogPath);

        public ParseController()
        {
        }

        /// <summary>
        /// Parses the files with the current installed version of Elite Insights.
        /// </summary>
        /// <param name="fileNames">The files to parse. The full path is required.</param>
        /// <returns>A list of strings containing the paths for the generated .json and .html files for each parsed file.</returns>
        public static IEnumerable<string> Parse(IEnumerable<string> fileNames)
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

            process.Start();
            process.WaitForExit();

            return Directory.EnumerateFiles(logPath);
        }

        public static bool IsInstalled()
        {
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(assemblyLocation, BaseEIPath);
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

            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(assemblyLocation, BaseEIPath, EliteInsightsExecutable);

            return FileVersionInfo.GetVersionInfo(path);
        }

        private static string GetConfig(string path)
        {
            string defaultConfig = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), EliteInsightsConfig);
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
