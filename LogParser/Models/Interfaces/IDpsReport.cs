using RestEase;
using System.Net.Http;
using System.Threading.Tasks;

namespace LogParser.Models.Interfaces
{
    public interface IDpsReport
    {
        [Post("uploadContent?json=1&generator=ei")]
        Task<DPSReport> UploadContent([Body] HttpContent file, [Query] string userToken);

        [Get("getUserToken")]
        Task<string> GetUserToken();
    }
}
