using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FilmLibrary.Migrations
{
    public partial class initialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Released = table.Column<string>(nullable: true),
                    Poster = table.Column<string>(nullable: true),
                    Plot = table.Column<string>(nullable: true),
                    Director = table.Column<string>(nullable: true),
                    Rating = table.Column<string>(nullable: true),
                    ImdbId = table.Column<string>(nullable: true),
                    Actors = table.Column<string>(nullable: true),
                    Production = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies_Actors",
                columns: table => new
                {
                    MovieId = table.Column<Guid>(nullable: false),
                    ActorId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies_Actors", x => new { x.MovieId, x.ActorId });
                    table.UniqueConstraint("AK_Movies_Actors_ActorId_MovieId", x => new { x.ActorId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_Movies_Actors_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Movies_Actors_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movies_Genres",
                columns: table => new
                {
                    MovieId = table.Column<Guid>(nullable: false),
                    GenreId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies_Genres", x => new { x.MovieId, x.GenreId });
                    table.UniqueConstraint("AK_Movies_Genres_GenreId_MovieId", x => new { x.GenreId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_Movies_Genres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Movies_Genres_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movies_Productions",
                columns: table => new
                {
                    MovieId = table.Column<Guid>(nullable: false),
                    ProductionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies_Productions", x => new { x.MovieId, x.ProductionId });
                    table.ForeignKey(
                        name: "FK_Movies_Productions_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Movies_Productions_Productions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users_Movies",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    MovieId = table.Column<Guid>(nullable: false),
                    Category = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_Movies", x => new { x.UserId, x.MovieId, x.Category });
                    table.UniqueConstraint("AK_Users_Movies_Category_MovieId_UserId", x => new { x.Category, x.MovieId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Users_Movies_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Movies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Productions_ProductionId",
                table: "Movies_Productions",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Movies_MovieId",
                table: "Users_Movies",
                column: "MovieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies_Actors");

            migrationBuilder.DropTable(
                name: "Movies_Genres");

            migrationBuilder.DropTable(
                name: "Movies_Productions");

            migrationBuilder.DropTable(
                name: "Users_Movies");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Productions");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
