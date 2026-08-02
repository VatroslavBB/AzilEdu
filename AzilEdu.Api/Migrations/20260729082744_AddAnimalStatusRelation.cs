using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AzilEdu.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAnimalStatusRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimalStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalStatuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AnimalStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Dostupna za udomljenje" },
                    { 2, "Rezervirana" },
                    { 3, "Udomljena" },
                    { 4, "Na liječenju" }
                });

            // Novi strani ključ. Sve postojeće životinje privremeno dobivaju status 1.
            migrationBuilder.AddColumn<int>(
                name: "AnimalStatusId",
                table: "Animals",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            // Tko je prije bio IsAdopted = true, sada dobiva status "Udomljena" (Id = 3).
            migrationBuilder.Sql(
                "UPDATE \"Animals\" SET \"AnimalStatusId\" = 3 WHERE \"IsAdopted\" = 1;");

            migrationBuilder.DropColumn(
                name: "IsAdopted",
                table: "Animals");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_AnimalStatusId",
                table: "Animals",
                column: "AnimalStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AnimalStatuses_AnimalStatusId",
                table: "Animals",
                column: "AnimalStatusId",
                principalTable: "AnimalStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // SQLite ignorira PRAGMA foreign_keys unutar transakcije, a EF svoju verziju
            // šalje baš unutar nje. Zato ga puštamo izvan transakcije, inače
            // DROP TABLE "AnimalStatuses" pukne na FK constraintu.
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdopted",
                table: "Animals",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            // Tko je imao status "Udomljena" (Id = 3), vraća se na IsAdopted = true.
            migrationBuilder.Sql(
                "UPDATE \"Animals\" SET \"IsAdopted\" = 1 WHERE \"AnimalStatusId\" = 3;");

            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AnimalStatuses_AnimalStatusId",
                table: "Animals");

            migrationBuilder.DropIndex(
                name: "IX_Animals_AnimalStatusId",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "AnimalStatusId",
                table: "Animals");

            migrationBuilder.DropTable(
                name: "AnimalStatuses");

            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }
    }
}
