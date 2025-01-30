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
    
    public static RecipeProjection MapToProjection(Recipe recipe)
    {
        return new RecipeProjection
        {
            RecipeId = recipe.RecipeId,
            Name = recipe.Name,
            Description = recipe.Description,
            Steps = recipe.Steps,
            Difficulty = recipe.Difficulty,
            Duration = recipe.Duration,
            NumberOfUpvotes = recipe.NumberOfUpvotes,
            NumberOfDownvotes = recipe.NumberOfDownvotes,
            NumberOfViews = recipe.NumberOfViews,
            ImageUrl = recipe.ImageUrl,
            TimeOfPublishement = recipe.TimeOfPublishement,
            IsPublished = recipe.IsPublished,
            UserId = recipe.UserId,
            Ingredients = recipe.Ingredients,
            Tags = recipe.Tags,
            Votes = recipe.Votes,
            Reviews = recipe.Reviews
        };
    }

    public async Task<bool> AddOrUpdateRecipe(Recipe recipe)
    {
        var projection = MapToProjection(recipe);
        var response = await _client.IndexAsync(projection, idx =>
            idx.Index(_elasticSettings.DefaultIndex)
                .OpType(OpType.Index));

        return response.IsValidResponse;
    }
    
    public async Task<Recipe> GetRecipe(string key)
    {
        var response = await _client.GetAsync<RecipeProjection>(key, g => g.Index(_elasticSettings.DefaultIndex));
    
        if (response.Source != null)
        {
            var recipe = new Recipe
            {
                RecipeId = response.Source.RecipeId,
                Name = response.Source.Name,
                Description = response.Source.Description,
                Steps = response.Source.Steps,
                Difficulty = response.Source.Difficulty,
                Duration = response.Source.Duration,
                NumberOfUpvotes = response.Source.NumberOfUpvotes,
                NumberOfDownvotes = response.Source.NumberOfDownvotes,
                NumberOfViews = response.Source.NumberOfViews,
                ImageUrl = response.Source.ImageUrl,
                TimeOfPublishement = response.Source.TimeOfPublishement,
                IsPublished = response.Source.IsPublished,
                UserId = response.Source.UserId,
                Ingredients = response.Source.Ingredients,
                Tags = response.Source.Tags,
                Votes = response.Source.Votes,
                Reviews = response.Source.Reviews
            };
            return recipe;
        }

        return null;
    }

    public async Task<IEnumerable<Recipe>> SearchRecipe(string query)
    {
        var searchRequest = new SearchRequest<RecipeProjection>(_elasticSettings.DefaultIndex)
        {
            Query = new MultiMatchQuery
            {
                Query = query,
                Fields = new[] { "name", "description" }
            }
        };

        var response = await _client.SearchAsync<RecipeProjection>(searchRequest);

        if (response.IsValidResponse)
        {
            return response.Documents.Select(projection => new Recipe
            {
                RecipeId = projection.RecipeId,
                Name = projection.Name,
                Description = projection.Description,
                Steps = projection.Steps,
                Difficulty = projection.Difficulty,
                Duration = projection.Duration,
                NumberOfUpvotes = projection.NumberOfUpvotes,
                NumberOfDownvotes = projection.NumberOfDownvotes,
                NumberOfViews = projection.NumberOfViews,
                ImageUrl = projection.ImageUrl,
                TimeOfPublishement = projection.TimeOfPublishement,
                IsPublished = projection.IsPublished,
                UserId = projection.UserId,
                Ingredients = projection.Ingredients,
                Tags = projection.Tags,
                Votes = projection.Votes,
                Reviews = projection.Reviews
            });
        }

        return new List<Recipe>();
    }

    public async Task<bool> RemoveRecipe(string key)
    {
        var response = await _client.DeleteAsync<Recipe>(key, g => g.Index(_elasticSettings.DefaultIndex));

        return response.IsValidResponse;
    }
    

}
