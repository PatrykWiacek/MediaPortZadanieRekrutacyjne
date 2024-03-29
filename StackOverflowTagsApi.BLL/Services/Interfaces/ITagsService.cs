using StackOverflowTagsApi.BLL.Models;

namespace StackOverflowTagsApi.BLL.Services.Interfaces
{
    public interface ITagsService
    {
        Task<List<Tag>> GetTagsFromStackOverflowApi();
        Task<List<Tag>> GetAllTags();
        Task<List<Tag>> GetPagedAndSortedTags(int page, int pageSize, string sortBy, bool sortAscending);
    }
}
