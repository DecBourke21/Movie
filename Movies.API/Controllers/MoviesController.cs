using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Movies.API.Models;
using Movies.Infrastructure.Interfaces;
using Movies.Infrastructure.Models;

namespace Movies.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ILogger<MoviesController> _logger;
        private readonly IMapper _mapper;
        private readonly IMovieRepository _repository;

        public MoviesController(ILogger<MoviesController> logger,
                                IMapper mapper, 
                                IMovieRepository repository)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
        }

        /// <summary>
        ///     Action to return the viewing statistics for all movies.
        /// </summary>
        /// <response code="200">Returned if the viewing statistics for all movies was successful</response>
        /// <response code="400">Returned if the viewing statistics for all movies could not be retrieved</response>
        /// <response code="401">Returned if the api key was invalid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        [Route("Stats")]
        public async Task<ActionResult> GetStats()
        {
            try
            {
                var stats = _repository.GetMovieStats();

                return Ok(_mapper.Map<List<MovieStats>, List<MovieStatsModel>>(stats));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return BadRequest("An error occurred while processing your request. Please try again and if the problem persists please contact support");
            }
        }
    }
}
