using Database.Models;
using LogParser.Models.Enums;
using System.Threading.Tasks;

namespace LogParser.Services
{
    public interface IParseService
    {
        /// <summary>
        /// Parses a single ArcDPS-LogFile.
        /// </summary>
        /// <param name="fileName">The name of the file to parse.</param>
        /// <param name="htmlPath">The path to folder to store the resulting Html-File.</param>
        /// <returns>The parsed log file.</returns>
        Task<ParsedLogFile> ParseSingleFile(string fileName, string htmlPath);

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
