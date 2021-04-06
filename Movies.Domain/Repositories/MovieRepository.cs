using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Movies.Infrastructure.Interfaces;
using Movies.Infrastructure.Models;

namespace Movies.Domain.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IMovieData _data;
        private readonly List<Metadata> _database;

        public MovieRepository(IMovieData data)
        {
            _data = data;
            _database = new List<Metadata>();
        }

        public List<Metadata> GetMetadata(int movieId)
        {
            var metadata = _data.GetMetadata();
            metadata.AddRange(_database);

            return metadata
                .Where(x => x.MovieId == movieId && 
                            movieId > 0 &&
                            !string.IsNullOrEmpty(x.Title?.Trim()) &&
                            !string.IsNullOrEmpty(x.Language?.Trim()) &&
                            !string.IsNullOrEmpty(x.Duration?.Trim()) &&
                            x.ReleaseYear.HasValue
                )
                .GroupBy(x => x.Language)
                .Select(x => x.OrderByDescending(m => m.Id).First())
                .OrderBy(x => x.Language)
                .ToList();
        }

        public bool AddMetadata(Metadata metadata)
        {
            _database.Add(metadata);

            return true;
        }

        public List<MovieStats> GetMovieStats()
        {
            var stats = _data.GetStats();

            return _data.GetMetadata().GroupBy(x => x.MovieId).Select(x => new MovieStats
            {
                MovieId = x.Key,
                Title = x.First().Title,
                ReleaseYear =  x.First().ReleaseYear,
                Watches = stats.Count(s => s.MovieId == x.Key && s.WatchDurationMs > 0),
                AverageWatchDurationS = stats.Any(s => s.MovieId == x.Key) ?
                    (int)((stats.Where(s => s.MovieId == x.Key).Sum(s => (long)s.WatchDurationMs) / stats.Count(s => s.MovieId == x.Key && s.WatchDurationMs > 0)) / 1000) :
                    0
            }).OrderByDescending(x => x.Watches).ThenByDescending(x => x.ReleaseYear).ToList();
        }
    }
}
