using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.Infrastructure.Models
{
    public class MovieStats
    {
        public int MovieId { get; set; }

        public string Title { get; set; }

        public int? AverageWatchDurationS { get; set; }

        public int? Watches { get; set; }

        public int? ReleaseYear { get; set; }
    }
}
