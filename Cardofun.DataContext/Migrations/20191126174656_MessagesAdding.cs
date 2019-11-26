using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cardofun.DataContext.Migrations
{
    public partial class MessagesAdding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_UserId_IsMain",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Photos");

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SenderId = table.Column<int>(nullable: false),
                    RecipientId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    PhotoId = table.Column<Guid>(nullable: true),
                    IsRead = table.Column<bool>(nullable: false),
                    ReadAt = table.Column<DateTime>(nullable: true),
                    SentAt = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    SenderDeleted = table.Column<bool>(nullable: false),
                    RecipientDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    PhotoId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsMain = table.Column<bool>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPhotos_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPhotos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_PhotoId",
                table: "Messages",
                column: "PhotoId",
                unique: true,
                filter: "[PhotoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPhotos_PhotoId",
                table: "UserPhotos",
                column: "PhotoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPhotos_UserId_IsMain",
                table: "UserPhotos",
                columns: new[] { "UserId", "IsMain" },
                unique: true,
                filter: "[IsMain] = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "UserPhotos");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "Photos",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Photos",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "Photos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Photos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId_IsMain",
                table: "Photos",
                columns: new[] { "UserId", "IsMain" },
                unique: true,
                filter: "[IsMain] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
