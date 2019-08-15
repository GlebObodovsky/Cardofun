using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cardofun.DataContext.Migrations
{
    public partial class InitMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Continents",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Continents", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    IsoCode = table.Column<string>(maxLength: 2, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    ContinentName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.IsoCode);
                    table.ForeignKey(
                        name: "FK_Countries_Continents_ContinentName",
                        column: x => x.ContinentName,
                        principalTable: "Continents",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 150, nullable: false),
                    CountryIsoCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryIsoCode",
                        column: x => x.CountryIsoCode,
                        principalTable: "Countries",
                        principalColumn: "IsoCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    CountryOfOriginCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Languages_Countries_CountryOfOriginCode",
                        column: x => x.CountryOfOriginCode,
                        principalTable: "Countries",
                        principalColumn: "IsoCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Login = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    Sex = table.Column<int>(nullable: false),
                    CityName = table.Column<string>(nullable: false),
                    Introduction = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2019, 8, 16, 0, 49, 13, 259, DateTimeKind.Local).AddTicks(2973)),
                    LastActive = table.Column<DateTime>(nullable: false),
                    PasswordHash = table.Column<byte[]>(nullable: false),
                    PasswordSalt = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Cities_CityName",
                        column: x => x.CityName,
                        principalTable: "Cities",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanguageLevel",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LanguageName = table.Column<string>(nullable: false),
                    LevelOfSpeaking = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageLevel", x => new { x.LanguageName, x.UserId });
                    table.ForeignKey(
                        name: "FK_LanguageLevel_Languages_LanguageName",
                        column: x => x.LanguageName,
                        principalTable: "Languages",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LanguageLevel_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LanguageLevel_Languages_LanguageName1",
                        column: x => x.LanguageName,
                        principalTable: "Languages",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LanguageLevel_Users_UserId1",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Url = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2019, 8, 16, 0, 49, 13, 269, DateTimeKind.Local).AddTicks(1996)),
                    IsMain = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryIsoCode",
                table: "Cities",
                column: "CountryIsoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ContinentName",
                table: "Countries",
                column: "ContinentName");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageLevel_UserId",
                table: "LanguageLevel",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageLevel_UserId1",
                table: "LanguageLevel",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CountryOfOriginCode",
                table: "Languages",
                column: "CountryOfOriginCode");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_Id",
                table: "Photos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId_IsMain",
                table: "Photos",
                columns: new[] { "UserId", "IsMain" },
                unique: true,
                filter: "[IsMain] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CityName",
                table: "Users",
                column: "CityName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguageLevel");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Continents");
        }
    }
}
