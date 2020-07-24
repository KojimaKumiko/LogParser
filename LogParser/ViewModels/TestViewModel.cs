using Microsoft.Win32;
using Stylet;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace LogParser.ViewModels
{
    public class TestViewModel : Screen
    {
        public TestViewModel()
        {
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
                using var arch = new ZipArchive(fs, ZipArchiveMode.Read);

                if (arch.Entries.Count != 1)
                {
                    Debug.WriteLine("Something is fishy");
                }

                using var data = arch.Entries[0].Open();
                using var ms = new MemoryStream();

                data.CopyTo(ms);
                ms.Position = 0;

                using var reader = new BinaryReader(ms, new System.Text.UTF8Encoding());
                string buildVersion = GetString(ms, 12);

                Debug.WriteLine($"Build Version: {buildVersion}");

                byte revision = reader.ReadByte();
                Debug.WriteLine($"Revision: {revision}");

                var id = reader.ReadUInt16();
                Debug.WriteLine($"ID: {id}");

                SafeSkip(ms, 1);

                // Agent Data
                // 4 bytes: player count
                int agentCount = reader.ReadInt32();

                for (int i = 0; i < agentCount; i++)
                {
                    ulong agent = reader.ReadUInt64();

                    uint prof = reader.ReadUInt32();

                    uint isElite = reader.ReadUInt32();

                    ushort toughness = reader.ReadUInt16();

                    ushort concentration = reader.ReadUInt16();

                    ushort healing = reader.ReadUInt16();

                    ushort hitboxWidth = reader.ReadUInt16();

                    ushort condition = reader.ReadUInt16();

                    ushort hitboxHeight = reader.ReadUInt16();

                    string name = GetString(ms, 68, false);
                }
            }
        }

        public void ParseFile()
        {
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
    }
}
