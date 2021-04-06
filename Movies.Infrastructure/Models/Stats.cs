using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace Movies.Infrastructure.Models
{
    public class Stats
    {
        [Name("movieId")]
        public int MovieId { get; set; }

        [Name("watchDurationMs")]
        public int? WatchDurationMs { get; set; }
    }
}
