using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models
{
    public class Specialization
    {
        public int ID { get; set; }
        
        public string Name { get; set; }
        
        public string Profession { get; set; }
        
        public bool Elite { get; set; }
        
        public string Icon { get; set; }
        
        public string Background { get; set; }

        [JsonProperty("profession_icon_big")]
        public string ProfessionIconBig { get; set; }
        
        [JsonProperty("profession_icon")]
        public string ProfessionIcon { get; set; }
    }
}
