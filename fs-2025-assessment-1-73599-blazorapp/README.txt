											  DublinBikes Blazor Client Application
====================================

This is the client-side Blazor application for the DublinBikes project, designed to consume the V2 API (CosmosDB implementation) and provide a master/detail view with full CRUD (Create, Read, Update, Delete) capabilities.

CLIENT DOCUMENTATION
--------------------

1. How to Configure the API Base URL
------------------------------------
The API base URL is configured in the appsettings.json file.

1. Open the file: fs-2025-assessment-1-73599-blazorapp/appsettings.json
2. Modify the "BaseUrl" value under the "ApiSettings" section to point to your running V2 API endpoint.

Example:
"ApiSettings": {
    "BaseUrl": "YOUR_API_ENDPOINT_HERE" 
    // e.g., "https://localhost:7253" or "https://your-api-url.azurewebsites.net"
}


2. How to Run the Blazor Project
--------------------------------
This project is a standard .NET 8 Blazor application.

1. Navigate to the project directory:
   cd fs-2025-assessment-1-73599-blazorapp
2. Run the application using the .NET CLI:
   dotnet run
3. The application will typically start on https://localhost:7001 (check the console output for the exact URL ).


3. Implemented Features
----------------------
The following core features are implemented in the client application:

- Master/Detail View: Stations list on /stations and individual detail view on /stations/{number}.
- Search & Filtering: Text search by Name/Address, Status filter (Open/Closed), and Minimum Available Bikes filter.
- Sorting: Stations can be sorted by Name, Available Bikes, and Occupancy (Ascending/Descending).
- Paging: Full pagination controls are implemented to handle large datasets.
- CRUD Operations: Create, Update, and Delete functionality is implemented via dedicated forms and buttons, using the StationsApiClient to communicate with the backend API.
