using System;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Movies.API.Controllers;
using Movies.API.Models;
using Movies.API.Profiles;
using Movies.Infrastructure.Interfaces;
using Movies.Infrastructure.Models;

namespace Movies.API.Test.Controllers
{
    [TestClass]
    public class MoviesControllerTests
    {
        private readonly MoviesController _controller;
        private readonly Mock<ILogger<MoviesController>> _logger;
        private readonly IMapper _mapper;
        private readonly Mock<IMovieRepository> _repository;

        public MoviesControllerTests()
        {
            _logger = new Mock<ILogger<MoviesController>>();
            _repository = new Mock<IMovieRepository>();

            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MoviesProfile());
            }).CreateMapper();

            _controller = new MoviesController(_logger.Object, _mapper, _repository.Object);
        }

        [TestMethod]
        public void Stats_WhenAnExceptionOccurs_ShouldReturnBadRequestWithGenericError()
        {
            string exceptionMessage = "Error";

            _repository.Setup(x => x.GetMovieStats()).Throws(new Exception(exceptionMessage));

            var result = _controller.GetStats().Result;

            (result as StatusCodeResult)?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            (result as BadRequestObjectResult)?.Value.Should().Be("An error occurred while processing your request. Please try again and if the problem persists please contact support");
        }

        [TestMethod]
        public void MoviesController_HasAuthorizeAttribute()
        {
            var authorizeAttribute = _controller.GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true);

            authorizeAttribute.Should().HaveCount(1, "because the Movies Controller should authorize the api key");
        }

        [TestMethod]
        public void GetStats_ShouldReturnOk()
        {
            _repository.Setup(x => x.GetMovieStats()).Verifiable();

            var result = _controller.GetStats().Result;

            (result as StatusCodeResult)?.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _repository.Verify();
        }

        [TestMethod]
        public void GetStats_ShouldReturnOkWithStats()
        {
            List<MovieStats> dbList = new List<MovieStats>
            {
                new MovieStats
                {
                    AverageWatchDurationS = 1,
                    MovieId = 1,
                    ReleaseYear = 1,
                    Title = "A",
                    Watches = 1
                },
                new MovieStats
                {
                    AverageWatchDurationS = 1,
                    MovieId = 1,
                    ReleaseYear = 1,
                    Title = "B",
                    Watches = 1
                }
            };

            _repository.Setup(x => x.GetMovieStats()).Returns(dbList);

            var result = _controller.GetStats().Result as OkObjectResult;

            result.Should().NotBeNull();

            var returnList = result?.Value as List<MovieStatsModel>;

            var expectedList = _mapper.Map<List<MovieStatsModel>>(dbList);

            returnList.Should().BeEquivalentTo(expectedList, " because GetStats should return the list from the repository");
        }
    }
}
