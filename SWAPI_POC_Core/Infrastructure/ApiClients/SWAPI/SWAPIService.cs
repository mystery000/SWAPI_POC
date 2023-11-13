using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SWAPI_POC_Core.Configuration;
using SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI.Models;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI
{
    public class SWAPIService : ISwapiService
    {
        private readonly SWAPISettings _sWAPISettings;
        private readonly string apiUrl;
        private enum HttpMethod
        {
            GET,
            POST
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="sWAPISettings">The SWAPI settings injected via dependency injection.</param>
        public SWAPIService(IOptions<SWAPISettings> sWAPISettings)
        {
            _sWAPISettings = sWAPISettings.Value;
            apiUrl = _sWAPISettings.BaseURL;
        }

        #region Private

        /// <summary>
        /// Sends an HTTP request to the specified URL using the specified HTTP method.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="httpMethod">The HTTP method to use for the request (GET or POST).</param>
        /// <returns>
        /// A string containing the response from the server.
        /// </returns>
        private async Task<string> Request(string url, HttpMethod httpMethod)
        {
            return await Request(url, httpMethod, null, false);
        }

        /// <summary>
        /// Sends an HTTP request to the specified URL using the specified HTTP method and data.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="httpMethod">The HTTP method to use for the request.</param>
        /// <param name="data">The data to include in the request body. Default is null.</param>
        /// <param name="isProxyEnabled">Whether to enable proxy for the request. Default is false.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the response from the server.
        /// </returns>
        private async Task<string> Request(string url, HttpMethod httpMethod, string data, bool isProxyEnabled)
        {
            string result = string.Empty;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = httpMethod.ToString();


            if (data != null) {
                byte[] bytes = UTF8Encoding.UTF8.GetBytes(data.ToString());
                httpWebRequest.ContentLength = bytes.Length;
                Stream stream = httpWebRequest.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Dispose();
            }

            var httpWebResponse = await httpWebRequest.GetResponseAsync();
            StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream());
            result = reader.ReadToEnd();
            reader.Dispose();

            return result;
        }

        /// <summary>
        /// Serializes a dictionary of key-value pairs into a URL-encoded string.
        /// </summary>
        /// <param name="dictionary">The dictionary to be serialized.</param>
        /// <returns>
        /// A URL-encoded string representation of the dictionary.
        /// </returns>
        private string SerializeDictionary(Dictionary<string, string> dictionary)
        {
            StringBuilder parameters = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
            {
                parameters.Append(keyValuePair.Key + "=" + keyValuePair.Value + "&");
            }
            return parameters.Remove(parameters.Length - 1, 1).ToString();
        }

        /// <summary>
        /// Retrieves a single entity of type T from the SWAPI API based on the specified endpoint and parameters.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="endpoint">The API endpoint for the entity.</param>
        /// <param name="parameters">Optional query parameters for the request.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the retrieved entity.
        /// </returns>
        private async Task<T> GetSingle<T>(string endpoint, Dictionary<string, string> parameters = null) where T : SharpEntity
        {
            string serializedParameters = "";
            if (parameters != null)
            {
                serializedParameters = "?" + SerializeDictionary(parameters);
            }

            return await GetSingleByUrl<T>(url: string.Format("{0}{1}{2}", apiUrl, endpoint, serializedParameters));
        }

        /// <summary>
        /// Retrieves multiple entities of type T from the SWAPI API based on the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="endpoint">The API endpoint for the entities.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the retrieved entities.
        /// </returns>
        private async Task<SharpEntityResults<T>> GetMultiple<T>(string endpoint) where T : SharpEntity
        {
            return await GetMultiple<T>(endpoint, null);
        }

        /// <summary>
        /// Retrieves multiple entities of type T from the SWAPI API based on the specified endpoint and parameters.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="endpoint">The API endpoint for the entities.</param>
        /// <param name="parameters">Optional query parameters for the request.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the retrieved entities.
        /// </returns>
        private async Task<SharpEntityResults<T>> GetMultiple<T>(string endpoint, Dictionary<string, string> parameters) where T : SharpEntity
        {
            string serializedParameters = "";
            if (parameters != null)
            {
                serializedParameters = "?" + SerializeDictionary(parameters);
            }

            string json = await Request(string.Format("{0}{1}{2}", apiUrl, endpoint, serializedParameters), HttpMethod.GET);
            SharpEntityResults<T> swapiResponse = JsonConvert.DeserializeObject<SharpEntityResults<T>>(json);

            return swapiResponse;
        }

        /// <summary>
        /// Extracts query parameters from a URL and returns them as a NameValueCollection.
        /// </summary>
        /// <param name="dataWithQuery">The URL containing the query parameters.</param>
        /// <returns>
        /// A NameValueCollection containing the extracted query parameters.
        /// </returns>
        private NameValueCollection GetQueryParameters(string dataWithQuery)
        {
            NameValueCollection result = new NameValueCollection();
            string[] parts = dataWithQuery.Split('?');
            if (parts.Length > 0)
            {
                string QueryParameter = parts.Length > 1 ? parts[1] : parts[0];
                if (!string.IsNullOrEmpty(QueryParameter))
                {
                    string[] p = QueryParameter.Split('&');
                    foreach (string s in p)
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(temp[0], temp[1]);
                        }
                        else
                        {
                            result.Add(s, string.Empty);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves all entities of type T from the SWAPI API in a specific page.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="entityName">The name of the entity to retrieve.</param>
        /// <param name="pageNumber">The page number of the results to retrieve. Default is 1.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the list of entities in the specified page.
        /// </returns>
        private async Task<SharpEntityResults<T>> GetAllPaginated<T>(string entityName, string pageNumber = "1") where T : SharpEntity
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("page", pageNumber);

            SharpEntityResults<T> result = await GetMultiple<T>(entityName, parameters);

            result.nextPageNo = String.IsNullOrEmpty(result.next) ? null : GetQueryParameters(result.next)["page"];
            result.previousPageNo = String.IsNullOrEmpty(result.previous) ? null : GetQueryParameters(result.previous)["page"];

            return result;
        }

        #endregion

        #region Public

        /// <summary>
        /// Retrieves a single entity of type T from the SWAPI API based on the specified URL.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="url">The URL of the entity to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the entity with the specified URL.
        /// </returns>
        public async Task<T> GetSingleByUrl<T>(string url) where T : SharpEntity
        {
            string json = await Request(url, HttpMethod.GET);
            T swapiResponse = JsonConvert.DeserializeObject<T>(json);
            return swapiResponse;
        }


        /// <summary>
        /// Retrieves all planets from the SWAPI API in a specific page.
        /// </summary>
        /// <param name="pageNumber">The page number of the results to retrieve. Default is 1.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the list of planets in the specified page.
        /// </returns>
        public async Task<SharpEntityResults<Planet>> GetAllPlanets(string? pageNumber = "1")
        {
            SharpEntityResults<Planet> result = await GetAllPaginated<Planet>("/planets/", pageNumber);

            return result;
        }

        /// <summary>
        /// Retrieves all species from the SWAPI API in a specific page.
        /// </summary>
        /// <param name="pageNumber">The page number of the results to retrieve. Default is 1.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the list of species in the specified page.
        /// </returns>
        public async Task<SharpEntityResults<Specie>> GetAllSpecies(string? pageNumber = "1")
        {
            SharpEntityResults<Specie> result = await GetAllPaginated<Specie>("/species/", pageNumber);

            return result;
        }


        /// <summary>
        /// Retrieves all films from the SWAPI API.
        /// </summary>
        /// <param name="pageNumber">The page number of the results to retrieve. Default is 1.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the list of films.
        /// </returns>
        public async Task<SharpEntityResults<Film>> GetAllFilms()
        {
            Stopwatch tw = Stopwatch.StartNew();
            string? pageNumber = "1";
            SharpEntityResults<Film> result = await GetAllPaginated<Film>("/films/", pageNumber);
            tw.Stop();
            Console.WriteLine($"Here {tw.ElapsedMilliseconds}");

            return result;
        }

        /// <summary>
        /// Retrieves a starship from the SWAPI API based on the specified ID.
        /// </summary>
        /// <param name="id">The ID of the starship to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the starship with the specified ID.
        /// </returns>
        public async Task<Starship> GetStarship(string id)
        {
            return await GetSingle<Starship>("/starships/" + id);
        }


        /// <summary>
        /// Retrieves a species from the SWAPI API based on the specified ID.
        /// </summary>
        /// <param name="id">The ID of the species to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the species with the specified ID.
        /// </returns>
        public async Task<Specie> GetSpecie(string id)
        {
            return await GetSingle<Specie>("/species/" + id);
        }


        /// <summary>
        /// Retrieves a film from the SWAPI API based on the specified ID.
        /// </summary>
        /// <param name="id">The ID of the film to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the film with the specified ID.
        /// </returns>
        public async Task<Film> GetFilm(int id)
        {
            return await GetSingle<Film>("/films/" + id);
        }


        /// <summary>
        /// Retrieves all starships from the SWAPI API.
        /// </summary>
        /// <param name="pageNumber">The page number of the results to retrieve. Default is 1.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the list of starships.
        /// </returns>
        public async Task<SharpEntityResults<Starship>> GetAllStarships(string? pageNumber = "1")
        {
            SharpEntityResults<Starship> result = await GetAllPaginated<Starship>("/starships/", pageNumber);

            return result;
        }

        /// <summary>
        /// Retrieves a list of people from the SWAPI API based on the specified person name.
        /// </summary>
        /// <param name="personName">The name of the person to search for.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the list of people matching the specified name.
        /// </returns>
        public async Task<SharpEntityResults<People>> GetPeopleByName(string personName)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"search", personName  }
            };
            return await GetMultiple<People>("people", parameters);
        }

        #endregion
    }
}
