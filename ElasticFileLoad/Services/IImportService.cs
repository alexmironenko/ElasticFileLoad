namespace ElasticFileLoad.Services
{
    /// <summary>
    /// File load main application functionality
    /// </summary>
    public interface IImportService
    {
        Task ImportAsync(string[] args);
        Task ImportFileAsync(string fullPath, bool isBulkLoad = false);
    }
}
