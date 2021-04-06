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
    public class MetadataControllerTests
    {
        private readonly MetadataController _controller;
        private readonly Mock<ILogger<MetadataController>> _logger;
        private readonly IMapper _mapper;
        private readonly Mock<IMovieRepository> _repository;
        private readonly int _movieId;
        private readonly Metadata _metadata;
        private readonly MetadataModel _metadataModel;

        public MetadataControllerTests()
        {
            _logger = new Mock<ILogger<MetadataController>>();
            _repository = new Mock<IMovieRepository>();

            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MoviesProfile());
            }).CreateMapper();

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
            _metadataModel = _mapper.Map<MetadataModel>(_metadata);

            _controller = new MetadataController(_logger.Object, _mapper, _repository.Object);
        }

        [TestMethod]
        public void Get_WhenAnExceptionOccurs_ShouldReturnBadRequestWithGenericError()
        {
            string exceptionMessage = "Error";

            _repository.Setup(x => x.GetMetadata(It.IsAny<int>())).Throws(new Exception(exceptionMessage));

            var result = _controller.Get(_movieId).Result;

            (result as StatusCodeResult)?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            (result as BadRequestObjectResult)?.Value.Should().Be("An error occurred while processing your request. Please try again and if the problem persists please contact support");
        }

        [TestMethod]
        public void MetadataController_HasAuthorizeAttribute()
        {
            var authorizeAttribute = _controller.GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true);

            authorizeAttribute.Should().HaveCount(1, "because the Metadata Controller should authorize the api key");
        }

        [TestMethod]
        public void Get_ShouldReturnOk()
        {
            _repository.Setup(x => x.GetMetadata(It.Is<int>(x => x == _movieId))).Returns(new List<Metadata> { _metadata }).Verifiable();

            var result = _controller.Get(_movieId).Result;

            (result as StatusCodeResult)?.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _repository.Verify();
        }

        [TestMethod]
        public void Get_ShouldReturnOkWithMetadata()
        {
            List<Metadata> dbList = new List<Metadata>
            {
                _metadata,
                new Metadata()
                {
                    Duration = "1",
                    Id = 1,
                    Language = "E",
                    MovieId = _movieId,
                    ReleaseYear = 1,
                    Title = "A"
                }
            };

            _repository.Setup(x => x.GetMetadata(It.Is<int>(x => x == _movieId))).Returns(dbList).Verifiable();

            var result = _controller.Get(_movieId).Result as OkObjectResult;

            result.Should().NotBeNull();

            var returnList = result?.Value as List<MetadataModel>;

            var expectedList = _mapper.Map<List<MetadataModel>>(dbList);

            returnList.Should().BeEquivalentTo(expectedList, " because Get should return the metadata from the repository");
        }

        [TestMethod]
        public void Get_ShouldReturnBadRequestWhenNullMetadata()
        {
            _repository.Setup(x => x.GetMetadata(It.Is<int>(x => x == _movieId))).Verifiable();

            var result = _controller.Get(_movieId).Result;

            (result as StatusCodeResult)?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            _repository.Verify();
        }

        [TestMethod]
        public void Get_ShouldReturnBadRequestWhenEmptyMetadata()
        {
            _repository.Setup(x => x.GetMetadata(It.Is<int>(x => x == _movieId))).Returns(new List<Metadata>()).Verifiable();

            var result = _controller.Get(_movieId).Result;

            (result as StatusCodeResult)?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            _repository.Verify();
        }

        [TestMethod]
        public void Post_WhenAnExceptionOccurs_ShouldReturnBadRequestWithGenericError()
        {
            string exceptionMessage = "Error";

            _repository.Setup(x => x.AddMetadata(It.IsAny<Metadata>())).Throws(new Exception(exceptionMessage));

            var result = _controller.Post(_metadataModel).Result;

            (result as StatusCodeResult)?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            (result as BadRequestObjectResult)?.Value.Should().Be("An error occurred while processing your request. Please try again and if the problem persists please contact support");
        }

        [TestMethod]
        public void Post_ShouldReturnOk()
        {
            _repository.Setup(x => x.AddMetadata(It.Is<Metadata>(x => x.MovieId == _metadata.MovieId && 
                                                                      x.Duration == _metadata.Duration && 
                                                                      x.Language == _metadata.Language && 
                                                                      x.ReleaseYear == _metadata.ReleaseYear && 
                                                                      x.Title == _metadata.Title))).Returns(true).Verifiable();

            var result = _controller.Post(_metadataModel).Result;

            (result as StatusCodeResult)?.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _repository.Verify();
        }

        [TestMethod]
        public void Post_ShouldReturnBadRequestWhenAddMetadataFailed()
        {
            _repository.Setup(x => x.AddMetadata(It.Is<Metadata>(x => x.MovieId == _metadata.MovieId &&
                                                                      x.Duration == _metadata.Duration &&
                                                                      x.Language == _metadata.Language &&
                                                                      x.ReleaseYear == _metadata.ReleaseYear &&
                                                                      x.Title == _metadata.Title))).Returns(false).Verifiable();
            var result = _controller.Post(_metadataModel).Result;

            (result as StatusCodeResult)?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            _repository.Verify();
        }
    }
}
