using RestEase;
using System.Threading.Tasks;

namespace LogParser.Models.Interfaces
{
    [Header("User-Agent", "LogParser")]
    public interface IGitHubApi
    {
        [Get("repos/{owner}/{repo}/releases/latest")]
        Task<GitHubRelease> GetLatestRelease([Path]string owner, [Path]string repo);
    }
}
