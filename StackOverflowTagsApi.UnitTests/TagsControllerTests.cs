using StackOverflowTagsApi.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using StackOverflowTagsApi.Tests.Helpers;

namespace StackOverflowTagsApi.Tests
{
    public class TagsControllerTests
    {
        
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

        private readonly TestApi _testApi;

        public TagsControllerTests()
        {
            _testApi = new TestApi();
        }
    }
}
