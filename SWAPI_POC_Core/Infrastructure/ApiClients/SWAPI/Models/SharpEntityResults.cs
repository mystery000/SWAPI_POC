namespace SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI.Models
{
    /// <summary>
    /// Represents the paginated results of a SharpEntity.
    /// </summary>
    /// <typeparam name="T">The type of SharpEntity.</typeparam>
    public class SharpEntityResults<T> : SharpEntity where T : SharpEntity
    {
        public string previous { get; set; } = string.Empty;


        public string next { get; set; } = string.Empty;


        public string previousPageNo { get; set; } = string.Empty;


        public string nextPageNo { get; set; } = string.Empty;


        public Int64 count { get; set; }


        public List<T> results { get; set; }

    }
}
