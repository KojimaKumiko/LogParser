using LogParser.Models;
using LogParser.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogParser.Services
{
    public interface IParseService
    {
        /// <summary>
        /// Parses a single ArcDPS-LogFile.
        /// </summary>
        /// <param name="fileNames">The path of the parsed JSON and HTML-Files.</param>
        /// <param name="htmlPath">The path to folder to store the resulting Html-File.</param>
        /// <returns>The parsed log file.</returns>
        ParsedLogFileDto ParseSingleFile(IEnumerable<string> fileNames, string htmlPath);

        /// <summary>
        /// Parses multiple ArcDPS-Log-Files with EliteInsights.
        /// </summary>
        /// <param name="fileNames">The path of the Log-Files</param>
        /// <param name="eiPath">The path for Elite Insights</param>
        /// <param name="logPath">The path for storing the resulting JSON and HTML-Files.</param>
        /// <returns>The full paths of the generated JSON and HTML-Files.</returns>
        Task<IEnumerable<string>> ParseFiles(IEnumerable<string> fileNames, string eiPath, string logPath);

        /// <summary>
        /// Checks whether Elite Insights is installed or not.
        /// </summary>
        /// <returns>True if Elite Insights is installed; otherwise false.</returns>
        bool IsInstalled();

        /// <summary>
        /// Get's the file version of Elite Insights.
        /// </summary>
        /// <returns>The file version of Elite Insights.</returns>
        string GetFileVersionInfo();

        /// <summary>
        /// Installs or Updates Elite Insights.
        /// </summary>
        /// <returns>Enum, indicating if it was installed, updated or an eror occurred.</returns>
        Task<Install> InstallEliteInsights();

        /// <summary>
        /// Clears the log folder created by Elite Insights.
        /// </summary>
        void ClearLogFolder();
    }
}
