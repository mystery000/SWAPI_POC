namespace SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI.Models
{
    public class Specie : SharpEntity
    {
        /// <summary>
        /// An array of vehicle resources that this person has piloted.
        /// </summary>
        public List<string> vehicles { get; set; }
     

        /// <summary>
        /// The URL of a planet resource, a planet that this species originates from.
        /// </summary>
        public string homeworld { get; set; } = string.Empty;
     

        /// <summary>
        /// A comma-seperated string of common eye colors for this species, none if this species does not typically have eyes.
        /// </summary>
        public string eye_colors { get; set; } = string.Empty;


        /// <summary>
        /// A comma-seperated string of common skin colors for this species, none if this species does not typically have skin.
        /// </summary>
        public string skin_colors { get; set; } = string.Empty;


        /// <summary>
        /// The hypermedia URL of this resource.
        /// </summary>
        public string url { get; set; } = string.Empty;


        /// <summary>
        /// the ISO 8601 date format of the time that this resource was created.
        /// </summary>
        public string created { get; set; } = string.Empty;


        /// <summary>
        /// the ISO 8601 date format of the time that this resource was edited.
        /// </summary>
        public string edited { get; set; } = string.Empty;


        /// <summary>
        /// An array of starship resources that this person has piloted.
        /// </summary>
        public List<string> starships { get; set; } 


        /// <summary>
        /// An array of Film URL Resources that this species has appeared in.
        /// </summary>
        public List<string> films { get; set; } 


        /// <summary>
        /// The url of the species resource that this person is.
        /// </summary>
        public List<string> people { get; set; }


        /// <summary>
        /// The classification of this species.
        /// </summary>
        public string classification { get; set; } = string.Empty;


        /// <summary>
        /// The name of this species.
        /// </summary>
        public string name { get; set; } = string.Empty;


        /// <summary>
        /// The designation of this species.
        /// </summary>
        public string designation { get; set; } = string.Empty;


        /// <summary>
        /// The average height of this person in centimeters.
        /// </summary>
        public string average_height { get; set; } = string.Empty;


        /// <summary>
        /// The average lifespan of this species in years.
        /// </summary>
        public string average_lifespane { get; set; } = string.Empty;


        /// <summary>
        /// A comma-seperated string of common hair colors for this species, none if this species does not typically have hair.
        /// </summary>
        public string hair_colors { get; set; } = string.Empty;

    }

}
