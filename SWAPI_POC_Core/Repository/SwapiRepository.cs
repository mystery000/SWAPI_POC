using SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI;
using SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI.Models;
using SWAPI_POC_Core.Interfaces;
using SWAPI_POC_Core.Models;


namespace SWAPI_POC_Core.Repository
{
    /// <summary>
    /// Represents a repository for accessing SWAPI data.
    /// </summary>
    public class SwapiRepository : ISwapiRepository
    {
        private readonly ISwapiService _swapiService;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="swapiService">The SWAPI service used for retrieving data.</param>
        public SwapiRepository(ISwapiService swapiService)
        {
            _swapiService = swapiService;
        }

        /// <summary>
        /// Retrieves a list of all species in the Star Wars films.
        /// </summary>
        /// <returns>
        /// A DataResponse object containing a list of species names.
        /// </returns>
        public async Task<DataResponse<List<string>>> GetAllSpecies()
        {
            DataResponse<List<string>> response = new();
            response.Data = new List<string>();

            var films = await _swapiService.GetAllFilms();

            if (films != null && films.results.Any())
            {
                var film = films.results.Where(x=>x.episode_id == 1).FirstOrDefault();
                var tasks = new List<Task<Specie>>();
                foreach (var specie in film.species)
                {
                    var slashArr = specie.Split("/");
                    var specieDetailTask = _swapiService.GetSpecie(slashArr[slashArr.Length - 2]);
                    tasks.Add(specieDetailTask);
                   
                }
                await Task.WhenAll(tasks);
                foreach (var specieDetailTask in tasks)
                {
                    var specieDetail = specieDetailTask.Result;
                    if (specieDetail != null)
                    {
                        response.Data.Add(specieDetail.classification);
                    }

                }
               
                response.Data = response.Data.Distinct().ToList();
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "Film not found";
            }

            return response;
        }

        /// <summary>
        /// Retrieves a list of starships associated with a person by their name.
        /// </summary>
        /// <param name="personName">The name of the person.</param>
        /// <returns>
        /// A DataResponse object containing a list of Starship objects.
        /// </returns>
        public async Task<DataResponse<List<Starship>>> GetAllStarshipsByPersonName(string personName = "Luke Skywalker")
        {
            DataResponse<List<Starship>> response = new();
            response.Data = new List<Starship>();

            var personDetail = await _swapiService.GetPeopleByName(personName);
            var tasks = new List<Task<Starship>>();
            if (personDetail != null && personDetail.results.Any())
            {
                var starships = personDetail.results.First().starships;
                
                foreach (var starship in starships)
                {
                    var slashArr = starship.Split("/");
                    var startShipResult = _swapiService.GetStarship(slashArr[slashArr.Length - 2]);
                    tasks.Add(startShipResult);
                   
                }
                await Task.WhenAll(tasks);
               
                foreach (var startShipTask in tasks)
                {
                    var startShipResult = startShipTask.Result;
                    if (startShipResult != null)
                    {
                        response.Data.Add(startShipResult);
                    }
                }
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "People not found";
            }

            return response;
        }

        /// <summary>
        /// Retrieves the total population of all planets in the SWAPI.
        /// </summary>
        /// <returns>
        /// A DataResponse object containing the total population as a string.
        /// </returns>
        public async Task<DataResponse<string>> GetTotalPopulation()
        {
            SharpEntityResults<Planet> planets = new SharpEntityResults<Planet>();
            DataResponse<string> response = new();
            Int64 totalPopulation = 0, populationPlanet = 0;
            string pageNumber = "1";
            planets = await _swapiService.GetAllPlanets(pageNumber);
            totalPopulation += planets.results.Where(item => Int64.TryParse(item.population, out populationPlanet))
                                                     .Sum(item => Convert.ToInt64(item.population));

            Int64 count = planets.count;
            if(count> 10)
            {
                decimal totalPages = Math.Ceiling((decimal)count / 10) - 1;
                var tasks = new List<Task<SharpEntityResults<Planet>>>();

                for (int i = 2; i < totalPages + 2; i++)
                {
                    var nextPlanets = _swapiService.GetAllPlanets(i.ToString());
                    tasks.Add(nextPlanets);
                }
                await Task.WhenAll(tasks);

                foreach (var task in tasks)
                {
                    var nextPlanets = task.Result;
                    if (nextPlanets != null)
                    {
                        totalPopulation += nextPlanets.results.Where(item => Int64.TryParse(item.population, out populationPlanet))
                                                                 .Sum(item => Convert.ToInt64(item.population));
                    }
                }
            
            }

            response.Success = true;
            response.Data = totalPopulation.ToString();
            response.Message = "Population not found";

            return response;
        }
    }
}
