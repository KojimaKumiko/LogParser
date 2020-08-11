using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models
{
    public class GitHubRelease
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public GitHubAsset[] Assets { get; set; }
    }

    public class GitHubAsset
    {
        public long ID { get; set; }

        public string Name { get; set; }

        [JsonProperty("browser_download_url")]
        public Uri DownloadUrl { get; set; }
    }
}
