
using SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI.Models;

namespace SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI
{
    public interface ISwapiService
    {
        Task<SharpEntityResults<Planet>> GetAllPlanets(string? pageNumber);

        Task<SharpEntityResults<Specie>> GetAllSpecies(string? pageNumber);

        Task<SharpEntityResults<Film>> GetAllFilms();

        Task<SharpEntityResults<People>> GetPeopleByName(string personName);

        Task<Specie> GetSpecie(string id);

        Task<Starship> GetStarship(string id);

        Task<Film> GetFilm(int id);

    }
}
