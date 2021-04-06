using Movies.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using Movies.Infrastructure.Models;

namespace Movies.Domain.Repositories
{
    public class MovieData : IMovieData
    {
        public List<Stats> GetStats()
        {
            using var reader = new StreamReader("stats.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Stats>().ToList();
        }

        public List<Metadata> GetMetadata()
        {
            using var reader = new StreamReader("metadata.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Metadata>().ToList();
        }
    }
}
