using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Movies.Domain.Repositories;
using Movies.Infrastructure.Interfaces;
using Movies.Infrastructure.Models;

namespace Movies.Domain.Test.Repositories
{
    [TestClass]
    public class MovieRepositoryTests
    {
        private readonly MovieRepository _repository;
        private readonly Mock<IMovieData> _movieData;
        private readonly int _movieId;
        private readonly Metadata _metadata;
        private readonly List<Metadata> _metadataList;
        private readonly Stats _stats;
        private readonly List<Stats> _statsList;

        public MovieRepositoryTests()
        {
            _movieData = new Mock<IMovieData>();

            _movieId = 3;
            _metadata = new Metadata
            {
                Duration = "1",
                Id = 1,
                Language = "L",
                MovieId = _movieId,
                ReleaseYear = 1,
                Title = "A"
            };
            _metadataList = new List<Metadata>
            {
                _metadata
            };
            _stats = new Stats
            {
                MovieId = _movieId,
                WatchDurationMs = 1
            };
            _statsList = new List<Stats>
            {
                _stats
            };
            _repository = new MovieRepository(_movieData.Object);
        }

        [TestMethod]
        public void GetMetadata_WhenCalled_ShouldReturnListOfMetadataForSpecifiedMovieId()
        {
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            _metadataList.Add(new Metadata { Duration = "1", Id = 2, Language = "L", MovieId = _movieId * 2, ReleaseYear = 1, Title = "B"});

            var result = _repository.GetMetadata(_movieId);

            var expectedList = _metadataList.Where(x => x.MovieId == _movieId).ToList();

            result.Should().BeEquivalentTo(expectedList, "because GetMetadata should only return the metadata for the specified id");
        }

        [TestMethod]
        public void GetMetadata_WhenCalled_ShouldReturnEmptyListWhenMovieIdIsZero()
        {
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            var result = _repository.GetMetadata(0);

            result.Should().BeEmpty("because movie id is zero.");
        }

        [TestMethod]
        public void GetMetadata_WhenCalled_ShouldExcludeMetadataWithoutDuration()
        {
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            var invalidMetadata = new Metadata
            {
                Duration = null,
                Id = 2,
                Language = "L",
                MovieId = _movieId,
                ReleaseYear = 1,
                Title = "B"
            };

            var result = _repository.GetMetadata(_movieId);

            result.Should().NotContain(invalidMetadata, "because GetMetadata should only return valid metadata");
        }

        [TestMethod]
        public void GetMetadata_WhenCalled_ShouldExcludeMetadataWithoutLanguage()
        {
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            var invalidMetadata = new Metadata
            {
                Duration = "1",
                Id = 2,
                Language = "",
                MovieId = _movieId,
                ReleaseYear = 1,
                Title = "B"
            };

            var result = _repository.GetMetadata(_movieId);

            result.Should().NotContain(invalidMetadata, "because GetMetadata should only return valid metadata");
        }

        [TestMethod]
        public void GetMetadata_WhenCalled_ShouldExcludeMetadataWithoutTitle()
        {
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            var invalidMetadata = new Metadata
            {
                Duration = "1",
                Id = 2,
                Language = "L",
                MovieId = _movieId,
                ReleaseYear = 1,
                Title = " "
            };

            var result = _repository.GetMetadata(_movieId);

            result.Should().NotContain(invalidMetadata, "because GetMetadata should only return valid metadata");
        }

        [TestMethod]
        public void GetMetadata_WhenCalled_ShouldExcludeMetadataWithoutReleaseYear()
        {
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            var invalidMetadata = new Metadata
            {
                Duration = "1",
                Id = 2,
                Language = "L",
                MovieId = _movieId,
                ReleaseYear = null,
                Title = "B"
            };

            var result = _repository.GetMetadata(_movieId);

            result.Should().NotContain(invalidMetadata, "because GetMetadata should only return valid metadata");
        }

        [TestMethod]
        public void GetMetadata_WhenCalled_ShouldGroupByLanguage()
        {
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            var metadata = new Metadata
            {
                Duration = _metadata.Duration,
                Id = _metadata.Id,
                Language = _metadata.Language,
                MovieId = _metadata.MovieId,
                ReleaseYear = _metadata.ReleaseYear,
                Title = _metadata.Title
            };

            _metadataList.Add(metadata);

            var result = _repository.GetMetadata(_movieId);

            var expectedList = _metadataList.GroupBy(x => x.Language).Select(x => x.First()).ToList();

            result.Should().BeEquivalentTo(expectedList, "because GetMetadata should group by language");
        }

        [TestMethod]
        public void GetMetadata_WhenCalled_ShouldGroupByLanguageAndOrderByLatestId()
        {
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            var metadata = new Metadata
            {
                Duration = _metadata.Duration,
                Id = _metadata.Id + 1,
                Language = _metadata.Language,
                MovieId = _metadata.MovieId,
                ReleaseYear = _metadata.ReleaseYear,
                Title = _metadata.Title
            };

            _metadataList.Add(metadata);

            var result = _repository.GetMetadata(_movieId);

            var expectedList = _metadataList.GroupBy(x => x.Language).Select(x => x.OrderByDescending(x => x.Id).First()).ToList();

            result.Should().BeEquivalentTo(expectedList, "because GetMetadata should group by language and order by id descending");
        }

        [TestMethod]
        public void GetMetadata_WhenCalled_ShouldReturnListOrderedByLanguage()
        {
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            _metadataList.Add(new Metadata
            {
                Duration = _metadata.Duration,
                Id = _metadata.Id + 1,
                Language = "B",
                MovieId = _metadata.MovieId,
                ReleaseYear = _metadata.ReleaseYear,
                Title = _metadata.Title
            });

            _metadataList.Add(new Metadata
            {
                Duration = _metadata.Duration,
                Id = _metadata.Id + 1,
                Language = "AA",
                MovieId = _metadata.MovieId,
                ReleaseYear = _metadata.ReleaseYear,
                Title = _metadata.Title
            });

            _metadataList.Add(new Metadata
            {
                Duration = _metadata.Duration,
                Id = _metadata.Id + 1,
                Language = "AB",
                MovieId = _metadata.MovieId,
                ReleaseYear = _metadata.ReleaseYear,
                Title = _metadata.Title
            });

            var result = _repository.GetMetadata(_movieId);

            var expectedList = _metadataList.OrderBy(x => x.Language).ToList();

            result.Should().BeEquivalentTo(expectedList, "because GetMetadata should be ordered by language alphabetically");
        }

        [TestMethod]
        public void AddMetadata_WhenCalled_ShouldAddToDatabaseAndReturnTrue()
        {
            var result = _repository.AddMetadata(_metadata);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void GetStats_WhenCalled_ShouldReturnListOfStats()
        {
            _movieData.Setup(x => x.GetStats()).Returns(_statsList);
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            _metadataList.Add(new Metadata { Duration = "1", Id = 2, Language = "L", MovieId = _movieId * 2, ReleaseYear = 1, Title = "B" });

            var result = _repository.GetMovieStats();

            var expectedList = _metadataList.GroupBy(x => x.MovieId).ToList();

            result.Should().HaveCount(expectedList.Count, "because GetMovieStats should return list of stats grouped by movie id");
        }

        [TestMethod]
        public void GetStats_WhenCalled_ShouldReturnListOfStatsOrderedByMostWatches()
        {
            _movieData.Setup(x => x.GetStats()).Returns(_statsList);
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            _metadataList.Add(new Metadata { Duration = "1", Id = 2, Language = "L", MovieId = _movieId * 2, ReleaseYear = 1, Title = "B" });

            var result = _repository.GetMovieStats();

            result.Should().BeInDescendingOrder(x => x.Watches, "because GetMovieStats should return list of stats ordered by most watches");
        }

        [TestMethod]
        public void GetStats_WhenCalled_ShouldReturnListOfStatsOrderedByNewestReleaseAfterWatches()
        {
            _movieData.Setup(x => x.GetStats()).Returns(new List<Stats>());
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            _metadataList.Add(new Metadata { Duration = "1", Id = 2, Language = "L", MovieId = _movieId * 2, ReleaseYear = 2, Title = "B" });

            var result = _repository.GetMovieStats();

            result.Should().BeInDescendingOrder(x => x.ReleaseYear, "because GetMovieStats should return list of stats ordered by newest release when watches are the same");
        }

        [TestMethod]
        public void GetStats_WhenCalled_ShouldCalculateWatches()
        {
            _movieData.Setup(x => x.GetStats()).Returns(_statsList);
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            _statsList.Add(new Stats { MovieId = _movieId, WatchDurationMs = 1 });

            var result = _repository.GetMovieStats();

            result.FirstOrDefault(x => x.MovieId == _movieId).Watches.Should().Be(
                _statsList.Count(x => x.MovieId == _movieId && x.WatchDurationMs > 0),
                "because GetMovieStats should set the watches to the number of times that movie id is found in the stats");
        }

        [TestMethod]
        public void GetStats_WhenCalled_ShouldCalculateWatchesIgnoringZeroTime()
        {
            _movieData.Setup(x => x.GetStats()).Returns(_statsList);
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            _statsList.Add(new Stats { MovieId = _movieId, WatchDurationMs = 0 });

            var result = _repository.GetMovieStats();

            result.FirstOrDefault(x => x.MovieId == _movieId).Watches.Should().Be(
                _statsList.Count(x => x.MovieId == _movieId && x.WatchDurationMs > 0),
                "because GetMovieStats should set the watches to the number of times that movie id is found in the stats");
        }

        [TestMethod]
        public void GetStats_WhenCalled_ShouldCalculateAverageWatchDurationInSeconds()
        {
            _movieData.Setup(x => x.GetStats()).Returns(_statsList);
            _movieData.Setup(x => x.GetMetadata()).Returns(_metadataList);

            _statsList.Add(new Stats { MovieId = _movieId, WatchDurationMs = 1000 });
            _statsList.Add(new Stats { MovieId = _movieId, WatchDurationMs = 10000 });
            _statsList.Add(new Stats { MovieId = _movieId, WatchDurationMs = 100000 });

            var result = _repository.GetMovieStats();

            int expected = (int) ((_statsList.Where(s => s.MovieId == _movieId).Sum(s => (long) s.WatchDurationMs) /
                                   _statsList.Count(s => s.MovieId == _movieId && s.WatchDurationMs > 0)) / 1000);

            result.FirstOrDefault(x => x.MovieId == _movieId).AverageWatchDurationS.Should().Be(expected, "because GetMovieStats should set the average watch duration in seconds (rather than ms)");
        }
    }
}
