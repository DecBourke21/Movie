using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.API.Models
{
    public class MovieStatsModel
    {
        public int? MovieId { get; set; }

        public string Title { get; set; }

        public int? AverageWatchDurationS { get; set; }

        public int? Watches { get; set; }

        public int? ReleaseYear { get; set; }
    }
}
