using AutoMapper;
using Database.Models;
using LogParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            DisableConstructorMapping();
            CreateMap<DpsTarget, DpsTargetDto>().ReverseMap();
            CreateMap<LogPlayer, LogPlayerDto>().ReverseMap();
            CreateMap<ParsedLogFile, ParsedLogFileDto>().ReverseMap();
        }
    }
}
