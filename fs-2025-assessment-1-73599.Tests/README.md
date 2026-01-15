# Unit Tests for fs-2025-assessment-1-73599

This project contains comprehensive unit tests for the main API project using **xUnit** and **Moq** frameworks.

## Test Files

### 1. StationServiceTests.cs
Tests for the `StationService` class including:
- **GetAllStations**: Verify all stations are returned
- **GetStationByNumber**: Test retrieval by valid/invalid station numbers
- **QueryStations Filters**:
  - Filter by status
  - Filter by minimum available bikes
  - Search by station name
  - Search by address
- **QueryStations Sorting**:
  - Sort by name (ascending/descending)
  - Sort by available bikes
  - Sort by occupancy
- **Pagination**: Test page and pageSize parameters
- **GetSummary**: Verify summary statistics calculation
- **AddStation**: Test adding new stations
- **UpdateStation**: Test updating existing stations
- **Combined Filters**: Test multiple filters together

**Total Test Cases**: 20+

### 2. CosmosStationServiceTests.cs
Tests for the `CosmosStationService` class including:
- **QueryStationsAsync Filters**: Status, minimum bikes, name/address search
- **QueryStationsAsync Sorting**: Name, available bikes, occupancy
- **Pagination**: Verify paginated results with correct total counts
- **GetSummaryAsync**: Verify async summary calculation
- **Combined Filters**: Multiple filters applied together
- **Occupancy Sorting**: Test occupancy percentage sorting
- **Pagination Info**: Verify page and pageSize are returned correctly

**Total Test Cases**: 11+

### 3. StationModelTests.cs
Tests for the `Station` model class including:
- **ID Property**: Verify number to string conversion
- **DateTime Conversions**: Test Unix timestamp to DateTimeOffset conversion
- **Dublin Time Conversion**: Test Europe/Dublin timezone conversion
- **Property Assignment**: Verify all properties can be set and retrieved
- **Edge Cases**: Test with zero bikes and boundary conditions

**Total Test Cases**: 5+

## Running the Tests

### In Visual Studio:
1. Open Test Explorer (Test > Windows > Test Explorer)
2. Click "Run All Tests" or select specific tests
3. View results in the Test Explorer window

### Via Command Line:
```bash
dotnet test fs-2025-assessment-1-73599.Tests
```

### Run Specific Test Class:
```bash
dotnet test fs-2025-assessment-1-73599.Tests --filter "ClassName=fs_2025_assessment_1_73599.Tests.Services.StationServiceTests"
```

## Test Coverage

- **Service Logic**: Filter, sort, paginate, and CRUD operations
- **Data Models**: Property conversions and calculations
- **Edge Cases**: Invalid inputs, empty results, zero values
- **Async Operations**: Async/await patterns with mocked dependencies

## Dependencies

- **xUnit**: Testing framework
- **Moq**: Mocking framework for dependency injection
- **.NET 8.0**: Target framework

## Notes

- Tests use temporary JSON files in the system temp directory for isolation
- Cosmos DB tests use Moq to isolate database dependencies
- All tests are independent and can run in any order
- Test data is created per test to ensure isolation and repeatability
