using ElasticFileLoad.Models;
using ElasticFileLoad.Services;
using Microsoft.Extensions.Options;

namespace ElasticFileLoad.Tests
{
    public class ElasticServiceTests
    {
        private readonly ElasticSettings _elasticSettings = new ElasticSettings() 
        { 
            Url = "http://localhost:9200/",
            IndexName = "elasticfileloadtests",
        };
        private readonly ElasticService _elasticService;


        public ElasticServiceTests()
        {
            _elasticService = new ElasticService(Options.Create(_elasticSettings));
            _elasticService.DeleteIndexIfExistsAsync().Wait();
            _elasticService.CreateIndexIfNotExistsAsync().Wait();
        }

        [Fact]
        public async Task CreateOrUpdate_ShouldReturnTrue_WhenCreatedOrUpdatedSuccessfully()
        {
            // Arrange
            User expectedUser = CreateSingleUser(1);

            // Act
            var actualResult = await _elasticService.AddOrUpdateAsync(expectedUser);

            // Assert
            Assert.True(actualResult);
        }

        [Fact]
        public async Task CreateOrUpdate_ShouldCreateUser_WhenCreatedSuccessfully()
        {
            // Arrange
            int expectedId = 2;
            User expectedUser = CreateSingleUser(expectedId);
            await _elasticService.AddOrUpdateAsync(expectedUser);

            // Act
            var actualUser = await _elasticService.GetAsync(expectedId);

            // Assert
            Assert.NotNull(actualUser);
            Assert.Equal(expectedUser.Id, actualUser.Id);
            Assert.Equal(expectedUser.Name, actualUser.Name);
            Assert.Equal(expectedUser.BirthDate, actualUser.BirthDate);
            Assert.Equal(expectedUser.SocialScore, actualUser.SocialScore);
        }

        [Fact]
        public async Task CreateOrUpdate_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            int expectedId = 3;
            User expectedUser = CreateSingleUser(expectedId);
            await _elasticService.AddOrUpdateAsync(expectedUser);
            expectedUser.Name += "Updated";
            expectedUser.BirthDate.AddMinutes(1);
            expectedUser.SocialScore += 1;
            await _elasticService.AddOrUpdateAsync(expectedUser);

            // Act
            var actualUser = await _elasticService.GetAsync(expectedId);

            // Assert
            Assert.NotNull(actualUser);
            Assert.Equal(expectedUser.Id, actualUser.Id);
            Assert.Equal(expectedUser.Name, actualUser.Name);
            Assert.Equal(expectedUser.BirthDate, actualUser.BirthDate);
            Assert.Equal(expectedUser.SocialScore, actualUser.SocialScore);
        }

        [Fact]
        public async Task Get_ShouldReturnUser_WhenIdIsGiven()
        {
            // Arrange
            int expectedId = 4;
            User expectedUser = CreateSingleUser(expectedId);
            await _elasticService.AddOrUpdateAsync(expectedUser);

            // Act
            var actualUser = await _elasticService.GetAsync(expectedId);

            // Assert
            Assert.NotNull(actualUser);
            Assert.Equal(expectedUser.Id, actualUser.Id);
            Assert.Equal(expectedUser.Name, actualUser.Name);
            Assert.Equal(expectedUser.BirthDate, actualUser.BirthDate);
            Assert.Equal(expectedUser.SocialScore, actualUser.SocialScore);
        }

        [Fact]
        public async Task Remove_ShouldDeleteUser_WhenIdIsGiven()
        {
            // Arrange
            int expectedId = 4;
            User expectedUser = CreateSingleUser(expectedId);
            await _elasticService.AddOrUpdateAsync(expectedUser);
            var addedUser = await _elasticService.GetAsync(expectedId);

            // Act
            await _elasticService.RemoveAsync(expectedId);
            var actualUser = await _elasticService.GetAsync(expectedId);

            // Assert
            Assert.NotNull(addedUser);
            Assert.Null(actualUser);
        }


        #region Helpers

        private static User CreateSingleUser(int id)
        {
            return new User()
            {
                Id = id,
                Name = "User" + id.ToString(),
                BirthDate = new DateTime(2000 + id % 10, 1, 1),
                SocialScore = id + 0.01F
            };
        }

        private static User[] CreateMultipleUsers(int[] ids)
        {
            return ids.Select(CreateSingleUser).ToArray();
        }

        #endregion
    }
}