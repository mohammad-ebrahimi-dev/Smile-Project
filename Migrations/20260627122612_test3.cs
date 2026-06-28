using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmileProject.Migrations
{
    /// <inheritdoc />
    public partial class test3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCategorySentences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CategorySentenceId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCategorySentences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCategorySentences_CategorySentences_CategorySentenceId",
                        column: x => x.CategorySentenceId,
                        principalTable: "CategorySentences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCategorySentences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCategorySentences_CategorySentenceId",
                table: "UserCategorySentences",
                column: "CategorySentenceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCategorySentences_UserId",
                table: "UserCategorySentences",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCategorySentences");
        }
    }
}
