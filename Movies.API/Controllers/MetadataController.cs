using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    public class MetadataController : ControllerBase
    {
        private readonly ILogger<MetadataController> _logger;
        private readonly IMapper _mapper;
        private readonly IMovieRepository _repository;

        public MetadataController(ILogger<MetadataController> logger,
                                  IMapper mapper,
                                  IMovieRepository repository)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
        }

        /// <summary>
        ///     Action to return the metadata for a movie.
        /// </summary>
        /// <response code="200">Returned if the movie metadata was successful</response>
        /// <response code="400">Returned if the movie metadata could not be retrieved</response>
        /// <response code="401">Returned if the api key was invalid</response>
        /// <response code="404">Returned if no metadata can be found for the specified movie</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet(":movieId")]
        public async Task<ActionResult> Get(int movieId)
        {
            try
            {
                var metadata = _repository.GetMetadata(movieId);

                if (metadata == null || !metadata.Any())
                    return NotFound($"Unable to find metadata for movie id: {movieId}");

                return Ok(_mapper.Map<List<MetadataModel>>(metadata));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return BadRequest("An error occurred while processing your request. Please try again and if the problem persists please contact support");
            }
        }

        /// <summary>
        ///     Action to write a new log message.
        /// </summary>
        /// <param name="metadataModel">Model to create a new piece of metadata</param>
        /// <response code="200">Returned if the metadata was saved</response>
        /// <response code="400">Returned if the model couldn't be parsed or saved</response>
        /// <response code="401">Returned if the api key was invalid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]MetadataModel metadataModel)
        {
            try
            {
                bool success = _repository.AddMetadata(_mapper.Map<Metadata>(metadataModel));

                if (!success)
                    throw new Exception("Unable to save metadata");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return BadRequest("An error occurred while processing your request. Please try again and if the problem persists please contact support");
            }
        }
    }
}
