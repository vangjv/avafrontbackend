using AvaFront.API.Config;
using AvaFront.API.Services;
using Azure;
using AvaFront.Infrastructure.Extensions;
using AvaFront.AutoGen.Agents.AvaFront;
using AvaFront.AutoGen.Agents.Restaurant;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var kvurl = configuration["KeyVaultURL"];

IConfiguration Configuration;
Configuration = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
     .Build();
foreach (var item in configuration.AsEnumerable())
{
    if (!string.IsNullOrEmpty(item.Key))
    {
        Environment.SetEnvironmentVariable(item.Key, item.Value);
    }
}

var openAIUri = configuration["OpenAIUri"];
var openAIApiKey = Environment.GetEnvironmentVariable("OpenAIApiKey");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.SetupCosmosDb(Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<RestaurantAgent, RestaurantAgent>();
builder.Services.AddSingleton<AvaFrontAgent, AvaFrontAgent>();
builder.Services.AddSingleton<OpenAIService>(sp => new OpenAIService(new Uri(openAIUri), new AzureKeyCredential(openAIApiKey)));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapSwagger();
app.UseAuthorization();
app.EnsureCosmosDbIsCreated();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
