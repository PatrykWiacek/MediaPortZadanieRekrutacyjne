using Microsoft.AspNetCore.Mvc;
using StackOverflowTagsApi.BLL.Models;
using StackOverflowTagsApi.BLL.Services.Interfaces;

namespace StackOverflowTagsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagsController : ControllerBase
    {
        //TODO dodac logowanie i wymuszenie ponownego pobrania
        private readonly ILogger<TagsController> _logger;
        private readonly ITagsService _tagsService;

        public TagsController(ILogger<TagsController> logger, ITagsService tagsService)
        {
            _logger = logger;
            _tagsService = tagsService;
        }

        [HttpGet("all", Name = "GetAllTags")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all tags");
                var tags = await _tagsService.GetAllTags();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting all tags: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("paged", Name = "GetPagedAndSortedTags")]
        public async Task<ActionResult<List<Tag>>> GetPagedAndSortedTags(int page = 1, int pageSize = 10, string sortBy = "Name", bool sortAscending = true)
        {
            try
            {
                _logger.LogInformation($"Getting paged and sorted tags (Page: {page}, PageSize: {pageSize}, SortBy: {sortBy}, SortAscending: {sortAscending})");
                var tags = await _tagsService.GetPagedAndSortedTags(page, pageSize, sortBy, sortAscending);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                // Obsługa błędu
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("force-refresh", Name = "ForceRefreshTags")]
        public async Task<ActionResult> ForceRefreshTags()
        {
            try
            {
                _logger.LogInformation("Forcing refresh of tags from Stack Overflow API");
                await _tagsService.GetTagsFromStackOverflowApi();
                return Ok("Tags refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while refreshing tags: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
