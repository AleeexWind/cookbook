using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cookbook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Portions = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Day = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    MealType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: true),
                    MenuPlanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealSlots_MenuPlans_MenuPlanId",
                        column: x => x.MenuPlanId,
                        principalTable: "MenuPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealSlots_MenuPlanId",
                table: "MealSlots",
                column: "MenuPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuPlans_OwnerId",
                table: "MenuPlans",
                column: "OwnerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealSlots");

            migrationBuilder.DropTable(
                name: "MenuPlans");
        }
    }
}
