using MediatR;
using SWAPI_POC_Core.Models;
using Microsoft.AspNetCore.Mvc;
using SWAPI_POC.Resources.Swapi.Operations;
using SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI.Models;

namespace SWAPI_POC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SwapiController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="mediator">The Mediator used for sending requests and receiving responses.</param>
        /// <exception cref="ArgumentNullException">Thrown when mediator is null.</exception>
        public SwapiController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Retrieves all starships associated with a person name.
        /// </summary>
        /// <returns>
        /// The IActionResult representing the response.
        /// </returns>
        /// <param name="limit" example="[2,10]"></param>

        [HttpGet("GetAllStarshipsByPersonName")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(DataResponse<List<Starship>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAllStarshipsByPersonName([FromQuery] GetStarships.GetStarshipsCommand command)
        {
            GetStarships.QueryResponse response = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        /// <summary>
        /// Retrieves all species classifications.
        /// </summary>
        /// <returns>
        /// The IActionResult representing the response.
        /// </returns>
        [HttpGet("GetAllSpeciesClassification")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(DataResponse<List<string>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAllSpeciesClassification()
        {
            GetAllSpecies.QueryResponse response = await _mediator.Send(new GetAllSpecies.GetAllSpeciesCommand());
            return StatusCode(StatusCodes.Status200OK, response);
        }

        /// <summary>
        /// Retrieves the total population of all planets.
        /// </summary>
        /// <returns>
        /// <remarks>Return the total population of all planets in the Galaxy</remarks>
        /// The IActionResult representing the response.
        /// </returns>
        [HttpGet("GetTotalPopulation")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [ProducesResponseType(typeof(DataResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> GetTotalPopulationOfAllPlanets()
        {
            GetTotalPopulation.QueryResponse response = await _mediator.Send(new GetTotalPopulation.GetTotalPopulationCommand());
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}
