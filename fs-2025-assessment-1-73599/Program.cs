using fs_2025_assessment_1_73599.Endpoint;
using fs_2025_assessment_1_73599.Seeding;
using fs_2025_assessment_1_73599.Startup;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Register StationService with JSON data
builder.Services.AddStationService("Data/dublinbike.json");	// V1
builder.Services.AddCosmosStationService(builder.Configuration); // V2

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
