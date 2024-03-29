using System.Data.SQLite;
using Microsoft.Extensions.Logging;
using Moq;
using StackOverflowTagsApi.BLL.Helpers;
using StackOverflowTagsApi.BLL.Models;
using StackOverflowTagsApi.BLL.Services;

namespace StackOverflowTagsApi.UnitTests
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
    }
}
