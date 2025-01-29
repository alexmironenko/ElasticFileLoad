using ElasticFileLoad.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ElasticFileLoad.Services
{
    /// <summary>
    /// Service with main application functionality - execute file import.
    /// </summary>
    public class ImportService : IImportService
    {
        private readonly IElasticService _elasticService;
        private readonly ILogger _logger;

        public ImportService(IElasticService elasticService, ILoggerFactory loggerFactory)
        {
            _elasticService = elasticService;
            _logger = loggerFactory.CreateLogger<ImportService>();
        }

        public async Task ImportAsync(string[] args)
        {
            var filename = await extractAndCheckFilename(args);
            if (filename != null)
            {
                await _elasticService.CreateIndexIfNotExistsAsync();
                _logger.LogInformation($"Importing {filename}...");
                try
                {
                    await ImportFileAsync(filename);
                    _logger.LogInformation("Import completed");
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Import error");
                }
            }
        }

        public async Task ImportFileAsync(string filename, bool isBulkLoad = false)
        {
            var userList = await loadFile(filename);
            if (userList != null)
            {
                if (isBulkLoad)
                {
                    await _elasticService.AddOrUpdateBulkAsync(userList);
                }
                else
                {
                    foreach (var user in userList)
                    {
                        await _elasticService.AddOrUpdateAsync(user);
                    }
                }
            }
        }

        private async Task<List<User>> loadFile(string filename)
        {
            List<User> userList = null;
            try
            {
                var jsonData = await Task.Run(() => File.ReadAllText(filename));
                userList = JsonConvert.DeserializeObject<List<User>>(jsonData);
            }
            catch (JsonReaderException ex)
            {
                _logger.LogCritical(ex, "JSON format error");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "File read error");
            }
            return userList;
        }

        private async Task<string> extractAndCheckFilename(string[] args)
        {
            string filename = null;
            if (args.Length == 0)
            {
                _logger.LogCritical("File name is not specified in parameters");
            }
            else
            {
                var exists = await Task.Run(() => File.Exists(args[0]));
                if (exists)
                {
                    filename = args[0];
                }
                else
                {
                    _logger.LogCritical("File is not found");
                }
            }
            return filename;
        }

    }
}