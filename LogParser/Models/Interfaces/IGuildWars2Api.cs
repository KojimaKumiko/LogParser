using LogParser.Models;
using RestEase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogParser
{
    public interface IGuildWars2Api
    {
        [Get("professions")]
        Task<List<string>> GetProfessionsAsync();

        [Get("specializations")]
        Task<List<int>> GetSpecializationsAsync();

        [Get("specializations/{id}")]
        Task<Specialization> GetSpecializationAsync([Path] int id);
    }
}
