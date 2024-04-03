using System.Data.SQLite;
using Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using StackOverflowTagsApi.BLL.Helpers;
using StackOverflowTagsApi.BLL.Models;
using StackOverflowTagsApi.BLL.Services;

namespace StackOverflowTagsApi.Tests
{
    public class TagsServiceTests
    {
        [Fact]
        public async void GetAllTags_ReturnsNonEmptyList()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TagsService>>();
            var logger = loggerMock.Object;
            var connection = new SQLiteConnection("DataSource=LocalDB.db");
            var tagsService = new TagsService(logger, connection);

            // Act
            var tags = tagsService.GetAllTags().Result;

            // Assert
            Assert.NotNull(tags);
            Assert.IsType<List<Tag>>(tags);
            Assert.True(tags.Count > 0);
        }

        [Fact]
        public async Task GetPagedAndSortedTags_ReturnsSortedTags()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TagsService>>();
            var logger = loggerMock.Object;
            var connection = new SQLiteConnection("DataSource=LocalDB.db");
            var tagsService = new TagsService(logger, connection);

            // Act
            var sortedTagsAscending = await tagsService.GetPagedAndSortedTags(page: 1, pageSize: 5, sortBy: "Name", sortAscending: true);
            var sortedTagsDescending = await tagsService.GetPagedAndSortedTags(page: 1, pageSize: 5, sortBy: "Name", sortAscending: false);

            // Assert
            Assert.NotNull(sortedTagsAscending);
            Assert.NotNull(sortedTagsDescending);
            Assert.Equal(5, sortedTagsAscending.Count);
            Assert.Equal(5, sortedTagsDescending.Count);

            for (int i = 0; i < sortedTagsAscending.Count - 1; i++)
            {
                Assert.True(string.Compare(sortedTagsAscending[i].Name, sortedTagsAscending[i + 1].Name) <= 0);
            }

            for (int i = 0; i < sortedTagsDescending.Count - 1; i++)
            {
                Assert.True(string.Compare(sortedTagsDescending[i].Name, sortedTagsDescending[i + 1].Name) >= 0);
            }
        }

        [Fact]
        public async Task AddTagsToDatabase_AddsNewTagsToDatabase()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TagsService>>();
            var logger = loggerMock.Object;
            var connection = new SQLiteConnection("DataSource=LocalDB.db");
            var tagsService = new TagsService(logger, connection);
            var newTags = new List<Tag>
            {
                new Tag { Name = "NewTag1", Count = 100, PercentCount = 0 },
                new Tag { Name = "NewTag2", Count = 150, PercentCount = 0 }
            };

            // Act
            await tagsService.AddTagsToDatabase(newTags);

            // Assert
            using (var dbConnection = new SQLiteConnection("DataSource=LocalDB.db"))
            {
                dbConnection.Open();
                var count = await dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Tags WHERE Name IN ('NewTag1', 'NewTag2')");
                Assert.Equal(2, count);
                dbConnection.Close();
            }
        }

        [Fact]
        public void CalculateTagPercentages_CalculatesCorrectPercentage()
        {
            // Arrange
            var tagsService = new TagsService(null, null);
            var totalCount = 1000;
            var tagCount = 200;
            var expectedPercentage = 20.0;

            // Act
            var percentage = tagsService.CalculateTagPercentages(totalCount, tagCount);

            // Assert
            Assert.Equal(expectedPercentage, percentage);
        }

    }
}
