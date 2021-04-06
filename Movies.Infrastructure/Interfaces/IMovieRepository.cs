using System;
using System.Collections.Generic;
using System.Text;
using Movies.Infrastructure.Models;

namespace Movies.Infrastructure.Interfaces
{
    public interface IMovieRepository
    {
        List<Metadata> GetMetadata(int movieId);

        bool AddMetadata(Metadata metadata);

        List<MovieStats> GetMovieStats();
    }
}
