# Employee Management API

Mini Project 2 backend for the Employee Management System.

## Stack

- .NET 8 Web API
- Entity Framework Core with SQL Server
- BCrypt password hashing
- JWT Bearer authentication
- Swagger / OpenAPI

## Default URLs

- API: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`
- MP2 Frontend: open `../frontend/index.html` with Live Server

## Seeded Accounts

- `admin / admin123` -> `Admin`
- `viewer / viewer123` -> `Viewer`

## Run

1. Open a terminal in `EmployeeManagement.Api`
2. Run `dotnet restore`
3. Run `dotnet build`
4. Run `dotnet run`

Default connection string:

`Server=(localdb)\MSSQLLocalDB;Database=EmployeeManagementDashboardDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`

## Endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/employees`
- `GET /api/employees/{id}`
- `GET /api/employees/dashboard`
- `POST /api/employees`
- `PUT /api/employees/{id}`
- `DELETE /api/employees/{id}`

## Notes

- Protected endpoints use `Authorization: Bearer <token>`
- `Admin` can create, update, and delete employees
- `Viewer` can only read employee and dashboard data
- Search, filter, sort, and pagination are server-driven
- The root folder app remains the Mini Project 1 frontend
- The `frontend/` folder is the Mini Project 2 API-connected frontend
