namespace ElasticFileLoad.Models
{
    /// <summary>
    /// Parsed app settings for ElasticSearch connection
    /// </summary>
    public class ElasticSettings
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string IndexName { get; set; }
    }
}
