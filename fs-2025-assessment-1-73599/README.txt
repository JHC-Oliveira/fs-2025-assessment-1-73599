DUBLIN BIKE STATION API
=======================

Overview
--------
A .NET 8 Web API that simulates live bike station data.
Supports:
- V1: JSON/in-memory backend
- V2: Cosmos DB backend
Includes a background service that updates station availability every 10–20 seconds.

Setup
-----
1. Install .NET 8 SDK.
2. Configure appsettings.json with Cosmos DB:

   "CosmosDb": {
        "EndpointUri": "https://localhost:8081",
        "PrimaryKey": "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
        "DatabaseName": "BikeStationDb",
        "ContainerName": "Stations"
    }

3. In Program.cs register services:

   var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "dublinbike.json");
   builder.Services.AddStationService(jsonPath);
   builder.Services.AddCosmosStationService(builder.Configuration, jsonPath);
   builder.Services.AddHostedService<CosmosStationUpdateBackgroundService>();

4. Run with:
   dotnet run

Swagger UI available at https://localhost:7253

Endpoints (V1 - JSON/in-memory)
-------------------------------
GET    /api/v1/stations    -> filter/sort/paginate
GET    /api/v1/stations/summary   -> totals & status counts
GET    /api/v1/stations/{number}  -> single station
POST   /api/v1/stations           -> add station
PUT    /api/v1/stations/{number}  -> update station

Endpoints (V2)
--------------
GET    /api/v2/stations/query     -> filter/sort/paginate
GET    /api/v2/stations/summary   -> totals & status counts
GET    /api/v2/stations/{number}  -> single station
POST   /api/v2/stations           -> add station
PUT    /api/v2/stations/{number}  -> update station

Background Service
------------------
- Runs automatically when API starts.
- Every 10–20 seconds updates all stations with random capacity and availability.
- Enforces invariant: bike_stands = available_bikes + available_bike_stands.
- Persists changes via Cosmos DB service.

Design Notes
------------
- V1 for quick prototyping with JSON.
- V2 for persistence and scalability using Cosmos DB.
- BackgroundService simulates real-time feed.
- Repository pattern keeps endpoints clean and testable.