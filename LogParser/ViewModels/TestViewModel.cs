using Database;
using Database.Models;
using LogParser.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestEase;
using Stylet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LogParser.ViewModels
{
    public class TestViewModel : Screen
    {
        private readonly string clearFilterText = " --- NONE --- ";

        private string selectedBossFilter;

        public TestViewModel()
        {
            LogFiles = new BindableCollection<ParsedLogFile>();
            BossNameFilters = new BindableCollection<string>()
            {
                clearFilterText
            };
        }

        public BindableCollection<ParsedLogFile> LogFiles { get; set; }

        public BindableCollection<string> BossNameFilters { get; set; }

        public string SelectedBossFilter
        {
            get { return selectedBossFilter; }
            set { SetAndNotify(ref selectedBossFilter, value); }
        }

        public void SetFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = false
            };
            bool result = (bool)fileDialog.ShowDialog();

            if (result)
            {
                using var fs = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                if (fileDialog.FileName.EndsWith(".zevtc"))
                {
                    using var arch = new ZipArchive(fs, ZipArchiveMode.Read);

                    if (arch.Entries.Count != 1)
                    {
                        Debug.WriteLine("Something is fishy");
                    }

                    using var data = arch.Entries[0].Open();
                    using var ms = new MemoryStream();

                    data.CopyTo(ms);
                    ms.Position = 0;
                    ParseData(ms);
                }
                else
                {
                    ParseData(fs);
                }
            }
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

        public void ParseEI()
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = false
            };
            bool result = (bool)fileDialog.ShowDialog();

            if (!result)
            {
                return;
            }

            string basePath = @$"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\EliteInsights\";
            string config = GetConfig(basePath);

            string args = $"-p -c \"{config}\" \"{fileDialog.FileName}\"";
            var processInfo = new ProcessStartInfo
            {
                FileName = basePath + "GuildWars2EliteInsights.exe",
                WorkingDirectory = basePath,
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
        }

        public void WriteToDatabase()
        {
            using DatabaseContext db = new DatabaseContext();

            ParsedLogFile logFile = new ParsedLogFile { BossName = "Test", Recorder = "SomePlayer" };

            db.ParsedLogFiles.Add(logFile);
            db.SaveChanges();

            LogFiles.Add(logFile);

            if (!BossNameFilters.Contains(logFile.BossName))
            {
                BossNameFilters.Add(logFile.BossName);
            }
        }

        public void ReadFromDatabase()
        {
            using DatabaseContext db = new DatabaseContext();
            LogFiles.AddRange(db.ParsedLogFiles);

            List<string> bossNames = LogFiles.Select(l => l.BossName).Distinct().ToList();
            BossNameFilters.AddRange(bossNames.Except(BossNameFilters));
        }

        public void BossFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            if (!string.IsNullOrWhiteSpace(selectedBossFilter) && SelectedBossFilter.Equals(clearFilterText))
            {
                SelectedBossFilter = null;
            }
        }

        private void ParseData(Stream stream)
        {
            using var reader = new BinaryReader(stream, new System.Text.UTF8Encoding());
            string buildVersion = GetString(stream, 12);

            Debug.WriteLine($"Build Version: {buildVersion}");

            byte revision = reader.ReadByte();
            Debug.WriteLine($"Revision: {revision}");

            var id = reader.ReadUInt16();
            Debug.WriteLine($"ID: {id}");

            SafeSkip(stream, 1);

            // Agent Data
            // 4 bytes: player count
            int agentCount = reader.ReadInt32();
            List<AgentData> agents = new List<AgentData>();

            for (int i = 0; i < agentCount; i++)
            {
                AgentData agent = new AgentData
                {
                    Agent = reader.ReadUInt64(),
                    Prof = reader.ReadUInt32(),
                    IsElite = reader.ReadUInt32(),
                    Toughness = reader.ReadUInt16(),
                    Concentration = reader.ReadUInt16(),
                    Healing = reader.ReadUInt16(),
                    HitboxWidth = reader.ReadUInt16(),
                    Condition = reader.ReadUInt16(),
                    HitboxHeight = reader.ReadUInt16(),
                    Name = GetString(stream, 68, false),
                };

                if (agent.IsElite == 0xFFFFFFFF)
                {
                    if ((agent.Prof & 0xffff0000) == 0xffff0000)
                    {
                        Debug.WriteLine("Gadget");
                    }
                    else
                    {
                        Debug.WriteLine("NPC");
                    }
                }

                agents.Add(agent);
            }

            // Skill Data
            // 4 bytes: player count
            uint skillCount = reader.ReadUInt32();

            List<SkillData> skills = new List<SkillData>();
            // 68 bytes: each skill
            for (int i = 0; i < skillCount; i++)
            {
                SkillData skill = new SkillData
                {
                    SkillID = reader.ReadInt32(), // 4 bytes: Skill ID
                    Name = GetString(stream, 64), // 64 bytes: Skill Name
                };

                skills.Add(skill);
            }

            long cbtItemCount = (reader.BaseStream.Length - reader.BaseStream.Position) / 64;
            List<CombatData> combats = new List<CombatData>();
            for (long i = 0; i < cbtItemCount; i++)
            {
                CombatData combatData = new CombatData
                {
                    Time = reader.ReadUInt64(),
                    SrcAgent = reader.ReadUInt64(),
                    DstAgent = reader.ReadUInt64(),
                    Value = reader.ReadInt32(),
                    BuffDmg = reader.ReadInt32(),
                    OverstackValue = reader.ReadUInt32(),
                    SkillID = reader.ReadUInt32(),
                    SrcInstID = reader.ReadUInt16(),
                    DstInstID = reader.ReadUInt16(),
                    SrcMasterInstID = reader.ReadUInt16(),
                    DstMasterInstID = reader.ReadUInt16(),
                    IFF = reader.ReadByte(),
                    Buff = reader.ReadByte(),
                    Result = reader.ReadByte(),
                    IsActivation = reader.ReadByte(),
                    IsBuffRemove = reader.ReadByte(),
                    IsNinety = reader.ReadByte(),
                    IsFifty = reader.ReadByte(),
                    IsMoving = reader.ReadByte(),
                    IsStateChange = reader.ReadByte(),
                    IsFlanking = reader.ReadByte(),
                    IsShields = reader.ReadByte(),
                    IsOffcycle = reader.ReadByte(),
                    Pad = reader.ReadUInt32(),
                };

                combats.Add(combatData);
            }
        }

        private string GetString(Stream stream, int length, bool nullTerminated = true)
        {
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);
            if (nullTerminated)
            {
                for (int i = 0; i < length; ++i)
                {
                    if (bytes[i] == 0)
                    {
                        length = i;
                        break;
                    }
                }
            }
            return System.Text.Encoding.UTF8.GetString(bytes, 0, length);
        }

        private void SafeSkip(Stream stream, long bytesToSkip)
        {
            if (stream.CanSeek)
            {
                stream.Seek(bytesToSkip, SeekOrigin.Current);
            }
            else
            {
                while (bytesToSkip > 0)
                {
                    stream.ReadByte();
                    --bytesToSkip;
                }
            }
        }

        private static string GetConfig(string path)
        {
            var defaultConfig = @$"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\EliteInsightsConfig.conf";
            var destConfig = path + "EIConfig.conf";

            File.Copy(defaultConfig, destConfig, true);
            File.AppendAllLines(destConfig, new string[] { @$"OutLocation={path}logs\" });

            return destConfig;
        }
    }
}
