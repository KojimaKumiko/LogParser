using LogParser.Models;
using LogParser.Models.Interfaces;
using Newtonsoft.Json.Linq;
using RestEase;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LogParser.Services
{
    public class DpsReportService
    {
        private static readonly Uri DpsReportUri = new Uri(@"https://dps.report");
        private readonly IDpsReport dpsReportApi;

        public DpsReportService()
        {
            dpsReportApi = RestClient.For<IDpsReport>(DpsReportUri);
        }

        public async Task<DPSReport> UploadToDpsReport(string file, string userToken)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            var fileName = file.Split("\\").Last();
            var fileContent = await File.ReadAllBytesAsync(file).ConfigureAwait(false);

            using var multiPartContent = new MultipartFormDataContent("----myBoundary");
            using var byteArrayContent = new ByteArrayContent(fileContent);

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

        public async Task<string> GetUserToken()
        {
            var response = await dpsReportApi.GetUserToken().ConfigureAwait(false);
            
            if (string.IsNullOrWhiteSpace(response))
            {
                return string.Empty;
            }

            var job = JObject.Parse(response);
            return job.Value<string>("userToken");
        }
    }
}
