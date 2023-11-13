
using SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI.Models;
using SWAPI_POC_Core.Models;

namespace SWAPI_POC_Core.Interfaces
{
    public interface ISwapiRepository
    {
        Task<DataResponse<List<Starship>>> GetAllStarshipsByPersonName(string personName);

        Task<DataResponse<string>> GetTotalPopulation();

        Task<DataResponse<List<string>>> GetAllSpecies();
        
    }
}
