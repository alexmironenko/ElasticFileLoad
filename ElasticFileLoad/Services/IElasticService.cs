using ElasticFileLoad.Models;

namespace ElasticFileLoad.Services
{
    /// <summary>
    /// Basic ElasticSearch functionality for User entity
    /// </summary>
    public interface IElasticService
    {
        Task<bool> CreateIndexIfNotExistsAsync();
        Task<bool> DeleteIndexIfExistsAsync();
        Task<bool> AddOrUpdateAsync(User user);
        Task<bool> AddOrUpdateBulkAsync(IEnumerable<User> users);
        Task<User> GetAsync(int id);
        Task<List<User>> GetAllAsync();
        Task<bool> RemoveAsync(int id);
    }
}