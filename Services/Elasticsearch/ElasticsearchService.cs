using System.Text.Json;
using let_em_cook.Configuration;
using Microsoft.Extensions.Options;

namespace let_em_cook.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Configuration;
using let_em_cook.Models;

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticSettings _elasticSettings;

    public ElasticsearchService(IOptions<ElasticSettings> configuration)
    {
        _elasticSettings = configuration.Value;
        var settings = new ElasticsearchClientSettings(new Uri(_elasticSettings.Url))
            .DefaultIndex(_elasticSettings.DefaultIndex);
        
        _client = new ElasticsearchClient(settings);
        
        this.CreateIndexIfNotExists("recipes").Wait();
    }

    private async Task CreateIndexIfNotExists(string indexName)
    {
        if (!_client.Indices.Exists(indexName).Exists) 
            await _client.Indices.CreateAsync(indexName);
    }

    public async Task<bool> AddOrUpdateRecipe(Recipe recipe)
    {
        var response = await _client.IndexAsync(recipe, idx =>
            idx.Index(_elasticSettings.DefaultIndex)
                .OpType(OpType.Index));

        return response.IsValidResponse;
    }

    public async Task<Recipe> GetRecipe(string key)
    {
        var response = await _client.GetAsync<Recipe>(key, g => g.Index(_elasticSettings.DefaultIndex));
        
        return response.Source;
    }

    public async Task<IEnumerable<Recipe>> SearchRecipe(string query)
    {
        var searchRequest = new SearchRequest<Recipe>(_elasticSettings.DefaultIndex)
        {
            Query = new MultiMatchQuery
            {
                Query = query,
                Fields = new[] { "name", "description" }
            }
        };

        var response = await _client.SearchAsync<Recipe>(searchRequest);

        if (response.IsValidResponse)
        {
            return response.Documents;
        }

        return new List<Recipe>();
    }

    public async Task<bool> RemoveRecipe(string key)
    {
        var response = await _client.DeleteAsync<Recipe>(key, g => g.Index(_elasticSettings.DefaultIndex));

        return response.IsValidResponse;
    }
    

}
