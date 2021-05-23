using LogParser.Models.Interfaces;
using RestEase;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser
{
    public class Helper
    {
        public static void OpenLink(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Process.Start("explorer", path);
        }

        public static async Task<string> CheckForNewVersion()
        {
            string link = null;

            IGitHubApi githubApi = RestClient.For<IGitHubApi>(new Uri(@"https://api.github.com"));
            var repos = await githubApi.GetReleases("KojimaKumiko", "LogParser").ConfigureAwait(true);

            if (repos == null || repos.Count <= 0)
            {
                throw new InvalidOperationException();
            }

            var version = GetVersion();
            var latest = repos.First();

            if (!latest.TagName.Contains(version, StringComparison.InvariantCultureIgnoreCase))
            {
                link = latest.Assets.First().DownloadUrl.ToString();
            }

            return link;
        }

        public static string GetVersion()
        {
            return typeof(Helper).Assembly.GetName().Version.ToString(3);
        }
    }
}
