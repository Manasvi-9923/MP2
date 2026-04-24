# Employee Management System - Mini Project 2

## Student Details

- Name: Manasvi Rajendra Shewale
- Batch: B7

## Project Structure

- `EmployeeManagement.Api/` - .NET 8 Web API backend
- `frontend/` - API-connected frontend for Mini Project 2
- `EMS.Tests/` - NUnit and Moq backend test project

## Prerequisites

- .NET SDK 8.0 or later
- SQL Server / LocalDB
- `dotnet-ef` tool
- VS Code Live Server (or any static file server)

## How To Run The Backend

1. Open a terminal in `EmployeeManagement.Api`
2. Run `dotnet restore`
3. Run `dotnet ef database update`
4. Run `dotnet run`
5. Open Swagger at `http://localhost:5000/swagger`

## How To Run The Frontend

1. Keep the API running on `http://localhost:5000`
2. Open `frontend/index.html` with Live Server
3. Login using one of these seeded users:
   - `admin / admin123`
   - `viewer / viewer123`

## How To Run Tests

1. Open a terminal in `EMS.Tests`
2. Run `dotnet test`

## Notes

- The frontend uses `http://localhost:5000/api`
- Search, filter, sort, and pagination are handled by the backend API
- Role-based access is enforced by JWT authentication and authorization
