using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackOverflowTagsApi.BLL.Helpers;
using StackOverflowTagsApi.BLL.Models;
using StackOverflowTagsApi.BLL.Services.Interfaces;

namespace StackOverflowTagsApi.BLL.Services
{
    public class TagsService : ITagsService
    {
        private readonly ILogger<TagsService> _logger;
        private readonly SQLiteConnection _connection;

        public TagsService(ILogger<TagsService> logger, SQLiteConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        public async Task<List<Tag>> GetAllTags()
        {
            try
            {
                _connection.Open();
                var sql = "SELECT * FROM Tags";
                var tags = await _connection.QueryAsync<Tag>(sql);
                _connection.Close();
                return tags.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(string.Format("\"An error occurred while retrieving tags : {0}"),e.Message);
                return null;
            }
        }

        public async Task<List<Tag>> GetPagedAndSortedTags(int page, int pageSize, string sortBy, bool sortAscending)
        {
            try
            {
                var tags = await _connection.QueryAsync<Tag>($"SELECT * FROM Tags ORDER BY {sortBy} {(sortAscending ? "ASC" : "DESC")} LIMIT @PageSize OFFSET @Offset",
                    new { PageSize = pageSize, Offset = (page - 1) * pageSize });

                return tags.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(string.Format("An error occurred while retrieving paged and sorted tags: {0}"),e.Message);
                return null;
            }
            
        }

        public async Task<List<Tag>> GetTagsFromStackOverflowApi()
        {
            var tags = new List<Tag>();

            try
            {
                for (int pageNumber = 1; pageNumber < 11; pageNumber++)
                {
                    var apiTags = $"/2.3/tags?page={pageNumber}&pagesize=100&order=desc&sort=popular&site=stackoverflow";

                    string url = HttpClientHelper.ApiClient.BaseAddress + apiTags;
                    using (HttpResponseMessage response = await HttpClientHelper.ApiClient.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {

                            string jsonResponse = await GetDecompressedResponse(response);
                            var tagResponse = JsonConvert.DeserializeObject<TagResponse>(jsonResponse);
                            tags.AddRange(tagResponse.Items);
                        }
                        else
                        {
                            _logger.LogError(response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            await AddTagsToDatabase(tags);
            return tags;
        }

        private async Task AddTagsToDatabase(List<Tag> tags)
        {

            if (tags == null || !tags.Any())
                return;

            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    foreach (var tag in tags)
                    {
                        tag.PercentCount = CalculateTagPercentages(tags.Sum(t => t.Count), tag.Count);
                        var existingTag = await _connection.QueryFirstOrDefaultAsync<Tag>("SELECT * FROM Tags WHERE Name = @Name", new { Name = tag.Name }, transaction);

                        if (existingTag == null)
                        {
                            await _connection.ExecuteAsync("INSERT INTO Tags (Name, Count, PercentCount) VALUES (@Name, @Count, @PercentCount)", tag, transaction);
                        }
                        else
                        {
                            await _connection.ExecuteAsync("UPDATE Tags SET PercentCount = @PercentCount, Count = @Count WHERE Name = @Name", tag, transaction);
                        }
                    }

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred while adding tags to the database: {e.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }

        private double CalculateTagPercentages(int totalCount, int tagCount)
        {
            double percentage = (double)tagCount / totalCount * 100;
            return Math.Round(percentage, 2);
        }

        private async Task<string> GetDecompressedResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                Stream contentStream = await response.Content.ReadAsStreamAsync();

                if (response.Content.Headers.ContentEncoding.Any())
                {
                    foreach (var encoding in response.Content.Headers.ContentEncoding)
                    {
                        if (encoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                        {
                            contentStream = new GZipStream(contentStream, CompressionMode.Decompress);
                        }
                        else if (encoding.Equals("deflate", StringComparison.InvariantCultureIgnoreCase))
                        {
                            contentStream = new DeflateStream(contentStream, CompressionMode.Decompress);
                        }
                    }
                }

                using (StreamReader reader = new StreamReader(contentStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            else
            {
                throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
            }
        }

    }
}

public class TagResponse
{
    public List<Tag> Items { get; set; }
}