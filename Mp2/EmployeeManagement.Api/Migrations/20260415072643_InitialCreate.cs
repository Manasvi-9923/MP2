using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$vRojhxZWmUInP1sWdOccr.UP7H7MsJTCEsubFcn9oBeQBH9vpABjC", "Admin", "admin" },
                    { 2, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$6KV0ibwC2Fywk7FRNlRCDuWMWbj/wmLSQdUl87KkK0cnSUamIAjpq", "Viewer", "viewer" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "Department", "Designation", "Email", "FirstName", "JoinDate", "LastName", "Phone", "Salary", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "Software Engineer", "priya.menon@xyz.com", "Priya", new DateTime(2022, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Menon", "9876543210", 750000m, "Active", new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "Senior Engineer", "arjun.sharma@xyz.com", "Arjun", new DateTime(2021, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Sharma", "9876501234", 950000m, "Active", new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Utc), "HR", "HR Executive", "meera.iyer@xyz.com", "Meera", new DateTime(2020, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Iyer", "9898989898", 520000m, "Active", new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Marketing Specialist", "rohan.patil@xyz.com", "Rohan", new DateTime(2023, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Patil", "9812345678", 600000m, "Inactive", new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Accountant", "sneha.desai@xyz.com", "Sneha", new DateTime(2019, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Desai", "9822334455", 580000m, "Active", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 3, 7, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Operations Executive", "vikram.singh@xyz.com", "Vikram", new DateTime(2021, 7, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Singh", "9800011223", 540000m, "Inactive", new DateTime(2026, 3, 7, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "UI Developer", "anjali.kulkarni@xyz.com", "Anjali", new DateTime(2022, 3, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Kulkarni", "9876123456", 680000m, "Active", new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Content Strategist", "rahul.verma@xyz.com", "Rahul", new DateTime(2020, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Verma", "9811122233", 510000m, "Active", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), "HR", "Talent Partner", "kavya.rao@xyz.com", "Kavya", new DateTime(2021, 10, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Rao", "9890011223", 610000m, "Inactive", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Finance Manager", "sanjay.nair@xyz.com", "Sanjay", new DateTime(2018, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nair", "9800998877", 880000m, "Active", new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Operations Manager", "divya.gupta@xyz.com", "Divya", new DateTime(2019, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Gupta", "9877001122", 820000m, "Active", new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "QA Engineer", "manish.joshi@xyz.com", "Manish", new DateTime(2023, 2, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Joshi", "9865321470", 560000m, "Active", new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Brand Manager", "nidhi.kapoor@xyz.com", "Nidhi", new DateTime(2020, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Kapoor", "9844556677", 720000m, "Inactive", new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Financial Analyst", "amit.bose@xyz.com", "Amit", new DateTime(2022, 11, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Bose", "9833445566", 640000m, "Active", new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2026, 3, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Logistics Coordinator", "shruti.banerjee@xyz.com", "Shruti", new DateTime(2021, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Banerjee", "9822003344", 500000m, "Inactive", new DateTime(2026, 3, 16, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Username",
                table: "AppUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
