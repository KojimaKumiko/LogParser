using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models
{
    public class SummonerDTO
    {
        public string AccountId { get; set; }

        public int ProfileIconId { get; set; }

        public long RevisionDate { get; set; }

        public string Name { get; set; }

        public string Id { get; set; }

        public string Puuid { get; set; }

        public long SummonerLevel { get; set; }
    }
}
