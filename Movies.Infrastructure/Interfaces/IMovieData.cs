using System;
using System.Collections.Generic;
using System.Text;
using Movies.Infrastructure.Models;

namespace Movies.Infrastructure.Interfaces
{
    public interface IMovieData
    {
        public List<Stats> GetStats();

        public List<Metadata> GetMetadata();
    }
}
