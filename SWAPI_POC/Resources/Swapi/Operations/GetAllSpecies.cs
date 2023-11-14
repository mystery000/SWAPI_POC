using MediatR;
using SWAPI_POC_Core.Interfaces;
using SWAPI_POC_Core.Models;


namespace SWAPI_POC.Resources.Swapi.Operations
{
    /// <summary>
    /// Represents the operation to get all species.
    /// </summary>
    public class GetAllSpecies
    {
        /// <summary>
        /// Command to get all species.
        /// </summary>
        public class GetAllSpeciesCommand : IRequest<QueryResponse>
        {
        }

        /// <summary>
        /// Response for the command.
        /// </summary>
        public class QueryResponse : DataResponse<List<string>>
        {
        }

        /// <summary>
        /// Handler for the command.
        /// </summary>
        public class CommandHandler : IRequestHandler<GetAllSpeciesCommand, QueryResponse>
        {
            private readonly ISwapiRepository _swapiRepository;

            /// <summary>
            /// CTOR
            /// </summary>
            /// <param name="swapiRepository">The SWAPI repository used for retrieving species data.</param>
            /// <exception cref="ArgumentNullException">Thrown when swapiRepository is null.</exception>
            public CommandHandler(ISwapiRepository swapiRepository)
            {
                _swapiRepository = swapiRepository ?? throw new ArgumentNullException(nameof(swapiRepository));
            }

            /// <summary>
            /// Handles the GetAllSpeciesCommand and returns the QueryResponse.
            /// </summary>
            /// <param name="command">The GetAllSpeciesCommand to handle.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>
            /// The QueryResponse.
            /// </returns>
            public async Task<QueryResponse> Handle(GetAllSpeciesCommand command, CancellationToken cancellationToken)
            {
                QueryResponse response = new();


                var dataResponse = await _swapiRepository.GetAllSpecies();

                if (dataResponse.Success)
                {
                    response.Success = true;
                    response.Data = dataResponse.Data;
                    response.Message = "Success";
                }
                else
                {
                    response.Success = false;
                    response.Message = dataResponse.Message;
                }


                return response;

            }
        }
    }
}
