using LogParser.Models;
using LogParser.Models.Interfaces;
using Microsoft.Xaml.Behaviors.Media;
using Newtonsoft.Json.Linq;
using RestEase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LogParser.Controller
{
    public class DpsReportController
    {
        private static readonly Uri dpsReportUri = new Uri(@"https://dps.report");

        public DpsReportController()
        {
        }

        public static async Task<DPSReport> UploadToDpsReport(string file, string userToken)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            IDpsReport dpsReportApi = RestClient.For<IDpsReport>(dpsReportUri);

            string fileName = file.Split("\\").Last();
            byte[] fileContent = await File.ReadAllBytesAsync(file).ConfigureAwait(false);

            using MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----myBoundary");
            using ByteArrayContent byteArrayContent = new ByteArrayContent(fileContent);

            byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
            multiPartContent.Add(byteArrayContent, "file", fileName);

            try
            {
                var response = await dpsReportApi.UploadContent(multiPartContent, userToken).ConfigureAwait(false);
                Debug.WriteLine(response);
                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static async Task<string> GetUserToken()
        {
            IDpsReport dpsReportApi = RestClient.For<IDpsReport>(dpsReportUri);
            var response = await dpsReportApi.GetUserToken().ConfigureAwait(false);
            
            if (string.IsNullOrWhiteSpace(response))
            {
                return string.Empty;
            }

            JObject job = JObject.Parse(response);
            return job.Value<string>("userToken");
        }
    }
}
