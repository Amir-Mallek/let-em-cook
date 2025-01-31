using System.Text;
using let_em_cook.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Azure.Storage.Blobs;
using let_em_cook.BackgroundServices;
using let_em_cook.Configuration;
using let_em_cook.Models;
using let_em_cook.Services;
using Microsoft.AspNetCore.Identity;
using let_em_cook.Data;
using let_em_cook.Services.Queues;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
// Add mailing and mail template services
builder.Services.AddSingleton<IEmailTemplateService,EmailTemplateService>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddControllers();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = true; // Optional, for pretty formatting
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Let Em Cook API", Version = "v1" });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}' (without quotes)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

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