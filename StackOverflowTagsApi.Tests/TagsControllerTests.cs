using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using StackOverflowTagsApi.BLL.Models;
using StackOverflowTagsApi.Tests.Helpers;

namespace StackOverflowTagsApi.Tests
{
    public class TagsControllerTests
    {
        private readonly TestApi _testApi;

        public TagsControllerTests()
        {
            _testApi = new TestApi();
        }
        
        [Fact]
        public async Task GetAllTags_ReturnsSuccessAndListOfTags()
        {
            // Act
            var response = await _testApi.Client.GetAsync("/Tags/all");

            // Assert
            response.EnsureSuccessStatusCode();
            var tags = await response.Content.ReadAsStringAsync();
            Assert.NotNull(tags);
            Assert.NotEmpty(tags);
        }

        [Fact]
        public async Task GetPagedAndSortedTags_ReturnsCorrectTags()
        {
            // Arrange
            var expectedPageSize = 100;
            var expectedPageNumber = 1; 
            var expectedSortBy = "Name"; 
            var expectedSortAscending = true; 

            // Act
            var response = await _testApi.Client.GetAsync($"/Tags/paged?page={expectedPageNumber}&pageSize={expectedPageSize}&sortBy={expectedSortBy}&sortAscending={expectedSortAscending}");
            response.EnsureSuccessStatusCode();
            var responseStream = await response.Content.ReadAsStreamAsync();
            var actualTags = await JsonSerializer.DeserializeAsync<List<Tag>>(responseStream);

            // Assert
            Assert.NotNull(actualTags);
            Assert.Equal(expectedPageSize, actualTags.Count);
        }

        [Fact]
        public async Task ForceRefreshTags_ReturnsSuccessAndRefreshesTags()
        {
            // Arrange
            var response = await _testApi.Client.PostAsync("/Tags/force-refresh", null);
            response.EnsureSuccessStatusCode();

            // Act
            var tagsResponse = await _testApi.Client.GetAsync("/Tags/all");
            tagsResponse.EnsureSuccessStatusCode();
            var tagsStream = await tagsResponse.Content.ReadAsStreamAsync();
            var refreshedTags = await JsonSerializer.DeserializeAsync<List<Tag>>(tagsStream);

            // Assert
            Assert.NotNull(refreshedTags);
            Assert.NotEmpty(refreshedTags);
        }
    }
}
