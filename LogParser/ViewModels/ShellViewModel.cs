using Microsoft.Win32;
using Stylet;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Media;

namespace LogParser.ViewModels
{
    public class ShellViewModel : Screen
    {
        public ShellViewModel()
        {
        }

        //public void DoSomething()
        //{
        //    OpenFileDialog fileDialog = new OpenFileDialog();
        //    fileDialog.Multiselect = false;
        //    bool result = (bool)fileDialog.ShowDialog();

        //    if (result)
        //    {
        //        using var fs = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        using var arch = new ZipArchive(fs, ZipArchiveMode.Read);

        //        if (arch.Entries.Count != 1)
        //        {
        //            Debug.WriteLine("Something is fishy");
        //        }

        //        using var data = arch.Entries[0].Open();
        //        using var ms = new MemoryStream();

        //        data.CopyTo(ms);
        //        ms.Position = 0;

        //        using var reader = new BinaryReader(ms, new System.Text.UTF8Encoding());
        //        string buildVersion = GetString(ms, 12); 

        //        Debug.WriteLine($"Build Version: {buildVersion}");

        //        byte revision = reader.ReadByte();
        //        Debug.WriteLine($"Revision: {revision}");

        //        var id = reader.ReadUInt16();
        //        Debug.WriteLine($"ID: {id}");

        //    }
        //}

        //private string GetString(Stream stream, int length, bool nullTerminated = true)
        //{
        //    byte[] bytes = new byte[length];
        //    stream.Read(bytes, 0, length);
        //    if (nullTerminated)
        //    {
        //        for (int i = 0; i < length; ++i)
        //        {
        //            if (bytes[i] == 0)
        //            {
        //                length = i;
        //                break;
        //            }
        //        }
        //    }
        //    return System.Text.Encoding.UTF8.GetString(bytes, 0, length);
        //}
    }
}
