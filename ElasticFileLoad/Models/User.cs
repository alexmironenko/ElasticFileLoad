using Newtonsoft.Json;

namespace ElasticFileLoad.Models
{
    /// <summary>
    /// Sample data class for abstract user to load to ElasticSearch
    /// </summary>
    public class User
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Dob")]
        public DateTime BirthDate { get; set; }

        [JsonProperty("Social")]
        public float SocialScore { get; set; }
    }
}