using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Movies.API.Models;
using Movies.Infrastructure.Models;

namespace Movies.API.Profiles
{
    public class MoviesProfile : Profile
    {
        public MoviesProfile()
        {
            CreateMap<MovieStatsModel, MovieStats>().ReverseMap();
            CreateMap<MetadataModel, Metadata>().ReverseMap();
        }
    }
}
