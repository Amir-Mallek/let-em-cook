using let_em_cook.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Azure.Storage.Blobs;
using let_em_cook.Services;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Add services to the container.
var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
var blobStorageConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");

builder.Services.AddSingleton(x => new BlobServiceClient(blobStorageConnectionString));
builder.Services.AddSingleton<BlobService>();

builder.Services.AddDbContext<ApplicationdbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();