using Elastic.Clients.Elasticsearch;
using ElasticFileLoad.Models;
using Microsoft.Extensions.Options;

namespace ElasticFileLoad.Services
{
    /// <summary>
    /// Service for basic ElasticSearch functionality for User entity
    /// </summary>
    public class ElasticService : IElasticService
    {
        private readonly ElasticsearchClient _client;
        private readonly ElasticSettings _elasticSettings;

        public ElasticService(IOptions<ElasticSettings> options)
        {
            _elasticSettings = options.Value;
            var settings = new ElasticsearchClientSettings(new Uri(_elasticSettings.Url))
#if DEBUG
                .ServerCertificateValidationCallback((sender, certificate, chain, errors) => { return true; })
#else
                .Authentication(new BasicAuthentication(_elasticSettings.Username, _elasticSettings.Password))
#endif
                .DefaultIndex(_elasticSettings.IndexName);
            _client = new ElasticsearchClient(settings);
        }

        public async Task<bool> CreateIndexIfNotExistsAsync()
        {
            if ((await _client.Indices.ExistsAsync(_elasticSettings.IndexName)).Exists)
            {
                return true;
            }
            var response = await _client.Indices.CreateAsync(_elasticSettings.IndexName);
            return response.IsSuccess();
        }

        public async Task<bool> DeleteIndexIfExistsAsync()
        {
            if (!(await _client.Indices.ExistsAsync(_elasticSettings.IndexName)).Exists)
            {
                return true;
            }
            var response = await _client.Indices.DeleteAsync(_elasticSettings.IndexName);
            return response.IsSuccess();
        }
 
        public async Task<bool> AddOrUpdateAsync(User user)
        {
            var response = await _client.IndexAsync(user, i => i
                .Index(_elasticSettings.IndexName)
                .Id(user.Id)
                .Refresh(Refresh.WaitFor));
            return response.IsValidResponse;
        }

        public async Task<bool> AddOrUpdateBulkAsync(IEnumerable<User> users)
        {
            var response = await _client.BulkAsync(b => b
                .Index(_elasticSettings.IndexName)
                .UpdateMany(users, (ud, u) => ud.Doc(u).DocAsUpsert(true)));
            return response.IsValidResponse;
        }

        public async Task<User> GetAsync(int id)
        {
            var response = await _client.GetAsync<User>(
                id.ToString(),
                g => g.Index(_elasticSettings.IndexName));
            return response.Source;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var response = await _client.SearchAsync<User>(s => s
                .Index(_elasticSettings.IndexName));
            return response.IsValidResponse ? response.Documents.ToList() : default;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var response = await _client.DeleteAsync<User>(
                id.ToString(),
                d => d.Index(_elasticSettings.IndexName));
            return response.IsValidResponse;
        }
    }
}