using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var loggerMock = new Mock<ILogger<TagsService>>();
            var logger = loggerMock.Object;
            var connection = new SQLiteConnection("DataSource=:memory:");
            connection.Open();

            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @"
             CREATE TABLE Tags (
                 Name TEXT NOT NULL,
                 Count INT NOT NULL,
                 PercentCount REAL NOT NULL
             );"; 
            createTableCommand.ExecuteNonQuery();
            // Arrange
            HttpClientHelper.InitializeClient();
            var tagsService = new TagsService(logger, connection);
            await tagsService.GetTagsFromStackOverflowApi();
            // Act
            var tags = tagsService.GetAllTags().Result;

            // Assert
            Assert.NotNull(tags);
            Assert.IsType<List<Tag>>(tags);
            Assert.True(tags.Count > 0);
        }
    }
}
