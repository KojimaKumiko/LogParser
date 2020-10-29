using RestEase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LogParser.Models.Interfaces
{
    public interface IDpsReport
    {
        [Post("uploadContent?json=1&generator=ei")]
        Task<DPSReport> UploadContent([Body] HttpContent file/*, [Query] string userToken*/);

        [Post("values")]
        Task<object> FormData([Body] HttpContent file);

        [Get("values/{id}")]
        Task<string> GetValues([Path] int id);
    }
}
