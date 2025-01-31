using System.Text;
using let_em_cook.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Azure.Storage.Blobs;
using let_em_cook.BackgroundServices;
using let_em_cook.Configuration;
using let_em_cook.Models;
using let_em_cook.Repositories;
using let_em_cook.Services;
using let_em_cook.Services.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using let_em_cook.Data;
using let_em_cook.Services.Queues;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Configure logging (you can adjust log levels and providers here)
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole(); // Add Console logging
builder.Logging.AddDebug(); // Add Debug logging

// Add services to the container.
var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
var blobStorageConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");

builder.Services.AddSingleton(x => new BlobServiceClient(blobStorageConnectionString));
builder.Services.AddSingleton<BlobService>();

builder.Services.AddScoped<IRepository<Vote>, Repository<Vote>>();
builder.Services.AddScoped<IRepository<Recipe>, Repository<Recipe>>();
builder.Services.AddScoped<CommentRepository>();
builder.Services.AddScoped<ReviewRepository>();

builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IVoteService, VoteService>();

builder.Services.AddDbContext<ApplicationdbContext>(options =>
    options.UseSqlServer(connectionString));
// Add Redis Connexion
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

// Add Queue Services
builder.Services.AddSingleton<IEmailQueueService, EmailQueueService>();
builder.Services.AddSingleton<IRecipePublicationQueueService, RecipePublicationQueueService>();

// Add Background Services
builder.Services.AddHostedService<EmailProcessingService>();
builder.Services.AddHostedService<RecipePublicationProcessingService>();
builder.Services.AddHostedService<ScheduledRecipePublisher>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = true; // Optional, for pretty formatting
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationdbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JwtBearerTokenSettings>(
    builder.Configuration.GetSection("JwtBearerTokenSettings"));
builder.Services.AddScoped<IRecipeService, RecipeService>();


var jwtSettings = builder.Configuration.GetSection("JwtBearerTokenSettings").Get<JwtBearerTokenSettings>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });
builder.Services.AddAuthorization();

builder.Services.Configure<ElasticSettings>(builder.Configuration.GetSection("Elasticsearch"));
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();