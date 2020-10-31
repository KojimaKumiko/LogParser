using RestEase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogParser.Models.Interfaces
{
    public interface ILeagueApi
    {
        [Get("Summoner/{region}/{name}")]
        Task<SummonerDTO> GetSummonerAsync([Path] string region, [Path] string name);
    }
}
