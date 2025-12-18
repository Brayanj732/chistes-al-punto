using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JokesAppByMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJokeFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RespuestaChiste",
                table: "JokeModel",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PreguntaChiste",
                table: "JokeModel",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "JokeModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "JokeModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                table: "JokeModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "JokeModel",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "JokeModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "RatingPromedio",
                table: "JokeModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRatings",
                table: "JokeModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Vistas",
                table: "JokeModel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "JokeModel");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "JokeModel");

            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "JokeModel");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "JokeModel");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "JokeModel");

            migrationBuilder.DropColumn(
                name: "RatingPromedio",
                table: "JokeModel");

            migrationBuilder.DropColumn(
                name: "TotalRatings",
                table: "JokeModel");

            migrationBuilder.DropColumn(
                name: "Vistas",
                table: "JokeModel");

            migrationBuilder.AlterColumn<string>(
                name: "RespuestaChiste",
                table: "JokeModel",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "PreguntaChiste",
                table: "JokeModel",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
