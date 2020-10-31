using LogParser.Models;
using LogParser.Models.Interfaces;
using RestEase;
using Stylet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace LogParser.ViewModels
{
    public class LeagueViewModel : Screen
    {
        private string selectedRegion;

        private string summonerName;

        private long summonerLevel;

        private string summonerIconSource;

        public LeagueViewModel()
        {
            Regions = new List<string> { "euw", "na", "jp", "oce", "eune" };
        }

        public List<string> Regions { get; private set; }

        public string SelectedRegion
        {
            get { return selectedRegion; }
            set { SetAndNotify(ref selectedRegion, value); }
        }

        public string SummonerName
        {
            get { return summonerName; }
            set { SetAndNotify(ref summonerName, value); }
        }

        public long SummonerLevel
        {
            get { return summonerLevel; }
            set { SetAndNotify(ref summonerLevel, value); }
        }

        public string SummonerIconSource
        {
            get { return summonerIconSource; }
            set { SetAndNotify(ref summonerIconSource, value); }
        }

        public async void SearchSummoner()
        {
            ILeagueApi leagueApi = RestClient.For<ILeagueApi>(@"https://localhost:44302/LeagueOfLegends");

            try
            {
                SummonerDTO summoner = await leagueApi.GetSummonerAsync(SelectedRegion, SummonerName).ConfigureAwait(true);
                if (summoner != null)
                {
                    SummonerLevel = summoner.SummonerLevel;
                    SummonerIconSource = string.Format(CultureInfo.InvariantCulture, @"http://ddragon.leagueoflegends.com/cdn/10.22.1/img/profileicon/{0}.png", summoner.ProfileIconId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
