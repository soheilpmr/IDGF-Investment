using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Migrations.SQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClaimDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimDefinitions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ClaimDefinitions",
                columns: new[] { "Id", "Description", "Type", "Value" },
                values: new object[,]
                {
                    { 1, "View invoices", "Permission", "CanViewInvoice" },
                    { 2, "Create invoices", "Permission", "CanCreateInvoice" },
                    { 3, "Delete invoices", "Permission", "DeleteUser" },
                    { 4, "Manage application users", "Permission", "RegisterUser" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimDefinitions");
        }
    }
}
