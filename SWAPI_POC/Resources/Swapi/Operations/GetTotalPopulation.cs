using MediatR;
using SWAPI_POC_Core.Interfaces;
using SWAPI_POC_Core.Models;

namespace SWAPI_POC.Resources.Swapi.Operations
{
    /// <summary>
    /// Represents the operation to get the total population.
    /// </summary>
    public class GetTotalPopulation
    {
        /// <summary>
        /// Command to get the total population.
        /// </summary>
        public class GetTotalPopulationCommand : IRequest<QueryResponse>
        { }

        /// <summary>
        /// Response for the command.
        /// </summary>
        public class QueryResponse : DataResponse<string>
        { 
        }

        /// <summary>
        /// Handler for the command.
        /// </summary>
        public class CommandHandler : IRequestHandler<GetTotalPopulationCommand, QueryResponse>
        {
            private readonly ISwapiRepository _swapiRepository;

            /// <summary>
            /// Initializes a new instance of the CommandHandler class with the specified ISwapiRepository.
            /// </summary>
            /// <param name="swapiRepository">The SWAPI repository used for retrieving data.</param>
            /// <exception cref="ArgumentNullException">Thrown when swapiRepository is null.</exception>
            public CommandHandler(ISwapiRepository swapiRepository)
            {
                _swapiRepository = swapiRepository ?? throw new ArgumentNullException(nameof(swapiRepository));
            }

            /// <summary>
            /// Handles the GetTotalPopulationCommand and returns the QueryResponse.
            /// </summary>
            /// <param name="command">The GetTotalPopulationCommand to handle.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>
            /// The QueryResponse.
            /// </returns>
            public async Task<QueryResponse> Handle(GetTotalPopulationCommand command, CancellationToken cancellationToken)
            {
                QueryResponse response = new();

                var baseResponse = await _swapiRepository.GetTotalPopulation();

                if (baseResponse.Success)
                {
                    response.Success = true;
                    response.Data = baseResponse.Data;
                    response.Message = "Success";
                }
                else
                {
                    response.Success = false;
                    response.Message = baseResponse.Message;
                }

                return response;

            }
        }
    }
}
