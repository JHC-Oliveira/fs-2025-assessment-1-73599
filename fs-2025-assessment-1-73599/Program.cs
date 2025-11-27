using fs_2025_assessment_1_73599.Endpoint;
using fs_2025_assessment_1_73599.Seeding;
using fs_2025_assessment_1_73599.Startup;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Path to JSON file
var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "dublinbike.json");

// Register StationService with JSON data
builder.Services.AddStationService(jsonPath);	// V1
builder.Services.AddCosmosStationService(builder.Configuration, jsonPath); // V2
builder.Services.AddStationUpdater(); // Background updater


// Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
	var cosmosClient = new CosmosClient(
		builder.Configuration["CosmosDb:EndpointUri"],
		builder.Configuration["CosmosDb:PrimaryKey"]);

	var seeder = new CosmosSeeder(cosmosClient,
		builder.Configuration["CosmosDb:DatabaseName"],
		builder.Configuration["CosmosDb:ContainerName"]);

	await seeder.SeedFromJsonAsync("Data/dublinbike.json");
	
}

// Map endpoints from Endpoint folder
app.MapStationEndpoints();

app.Run();
