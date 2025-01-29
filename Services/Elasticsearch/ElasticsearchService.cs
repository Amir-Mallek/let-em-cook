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
    private readonly string _indexName;

    public ElasticsearchService(IConfiguration configuration)
    {
        var settings = new ElasticsearchClientSettings(new Uri(configuration["Elasticsearch:Url"]));
        _client = new ElasticsearchClient(settings);
        _indexName = configuration["Elasticsearch:IndexName"];
    }

    public async Task IndexRecipeAsync(Recipe recipe)
    {
        await _client.IndexAsync(recipe, idx => idx.Index(_indexName));
    }

    public async Task<IEnumerable<Recipe>> SearchRecipesAsync(string query)
    {
        var response = await _client.SearchAsync<Recipe>(s => s
            .Index(_indexName)
            .Query(q => q.Match(m => m
                .Field(f => f.Name)
                .Query(query)
            ))
        );

        return response.Documents;
    }
}
