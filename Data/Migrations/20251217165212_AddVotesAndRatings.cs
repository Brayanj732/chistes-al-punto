using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JokesAppByMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVotesAndRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JokeRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JokeModelId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JokeRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JokeRatings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JokeRatings_JokeModel_JokeModelId",
                        column: x => x.JokeModelId,
                        principalTable: "JokeModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JokeVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JokeModelId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsLike = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JokeVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JokeVotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JokeVotes_JokeModel_JokeModelId",
                        column: x => x.JokeModelId,
                        principalTable: "JokeModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JokeRatings_JokeModelId_UserId",
                table: "JokeRatings",
                columns: new[] { "JokeModelId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JokeRatings_UserId",
                table: "JokeRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JokeVotes_JokeModelId_UserId",
                table: "JokeVotes",
                columns: new[] { "JokeModelId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JokeVotes_UserId",
                table: "JokeVotes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JokeRatings");

            migrationBuilder.DropTable(
                name: "JokeVotes");
        }
    }
}
