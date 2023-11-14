using SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI;
using SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI.Models;
using SWAPI_POC_Core.Interfaces;
using SWAPI_POC_Core.Models;
using System.Diagnostics;

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
            Stopwatch tw = Stopwatch.StartNew();
            var films = await _swapiService.GetAllFilms();

            if (films != null && films.results.Any())
            {
                var film = films.results.Where(x=>x.episode_id == 1).FirstOrDefault();

                if(film == null)
                {
                    response.Success = false;
                    response.Message = "Episode not found!";
                }
                else
                {

                    var tasks = film.species.Select(specie =>
                    {
                        var slashArr = specie.Split("/");
                        var specId = slashArr[slashArr.Length - 2];
                        return _swapiService.GetSpecie(specId);
                    });

                    var speciesDetails = await Task.WhenAll(tasks);

                    response.Data = speciesDetails
                                        .Where(specie => specie != null)
                                        .Select(specie => specie.classification)
                                        .Distinct()
                                        .ToList();

                    response.Success = true;
                }
            }
            else
            {
                response.Success = false;
                response.Message = "Film not found";
            }
            tw.Stop();
            Console.WriteLine($"Fetched all species {tw.ElapsedMilliseconds}(ms)");
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
            Stopwatch tw = Stopwatch.StartNew();
            var personDetail = await _swapiService.GetPeopleByName(personName);
            var tasks = new List<Task<Starship>>();
            if (personDetail != null && personDetail.results.Any())
            {
                var starships = personDetail.results.First().starships;

                IEnumerable<Task<Starship>> starshipTasks = starships.Select(starshipUrl =>
                {
                    var slashArr = starshipUrl.Split("/");
                    var starshipId = slashArr[slashArr.Length - 2];
                    return _swapiService.GetStarship(starshipId);
                });

                var starshipResults = await Task.WhenAll(starshipTasks);

                response.Data.AddRange(starshipResults.Where(result => result != null));
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "People not found";
            }
            tw.Stop();
            Console.WriteLine($"Fetched all starshipsByPersonName {tw.ElapsedMilliseconds}(ms)");
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
            Int64 totalPopulation = 0, populationPlanet = 0, pageSize = 10;
            string pageNumber = "1";
            Stopwatch tw = Stopwatch.StartNew();
            planets = await _swapiService.GetAllPlanets(pageNumber);
            totalPopulation += planets.results.Where(item => Int64.TryParse(item.population, out populationPlanet))
                                                     .Sum(item => Convert.ToInt64(item.population));

            Int64 count = planets.count;

            if(count > pageSize)
            {
                decimal totalPages = Math.Ceiling((decimal)count / pageSize);
                var tasks = new List<Task<SharpEntityResults<Planet>>>();

                for (int i = 2; i <= totalPages; i++)
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

            tw.Stop();
            Console.WriteLine($"Fetched total population {tw.ElapsedMilliseconds}(ms)");

            return response;
        }
    }
}
