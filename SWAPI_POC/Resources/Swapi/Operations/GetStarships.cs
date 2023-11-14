using MediatR;
using SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI.Models;
using SWAPI_POC_Core.Interfaces;
using SWAPI_POC_Core.Models;

namespace SWAPI_POC.Resources.Swapi.Operations
{
    /// <summary>
    /// Represents the operation to get starships by person name.
    /// </summary>
    public class GetStarships
    {
        /// <summary>
        /// Represents the command to get starships by person name.
        /// </summary>
        public class GetStarshipsCommand : IRequest<QueryResponse>
        {
            public string PersonName { get; set; } = "Luke Skywalker";
        }

        /// <summary>
        /// Represents the response of the command.
        /// </summary>
        public class QueryResponse : DataResponse<List<Starship>>
        { 
        }

        /// <summary>
        /// Represents the handler for the command.
        /// </summary>
        public class CommandHandler : IRequestHandler<GetStarshipsCommand, QueryResponse>
        {
            private readonly ISwapiRepository _swapiRepository;

            /// <summary>
            /// CTOR
            /// </summary>
            /// <param name="swapiRepository">The repository used for retrieving data from SWAPI.</param>
            /// <exception cref="ArgumentNullException">Thrown when swapiRepository is null.</exception>
            public CommandHandler(ISwapiRepository swapiRepository)
            {
                _swapiRepository = swapiRepository ?? throw new ArgumentNullException(nameof(swapiRepository));
            }

            /// <summary>
            /// Handles the GetStarshipsCommand by retrieving starships from SWAPI based on a person name.
            /// </summary>
            /// <param name="command">The command containing the person name.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>A task that represents the asynchronous operation and contains the response of the command.</returns>
            public async Task<QueryResponse> Handle(GetStarshipsCommand command, CancellationToken cancellationToken)
            {
                QueryResponse response = new();

                var dataResponse = await _swapiRepository.GetAllStarshipsByPersonName(command.PersonName);

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
