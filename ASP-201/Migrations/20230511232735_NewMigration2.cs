using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_201.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.DropForeignKey(
                name: "FK_Posts_Posts_ReplyId",
                table: "Posts");*/

            migrationBuilder.AlterColumn<Guid>(
                name: "ReplyId",
                table: "Posts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            /*migrationBuilder.AddForeignKey(
                name: "FK_Posts_Posts_ReplyId",
                table: "Posts",
                column: "ReplyId",
                principalTable: "Posts",
                principalColumn: "Id");*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_Posts_Posts_ReplyId",
                table: "Posts");*/

            migrationBuilder.AlterColumn<Guid>(
                name: "ReplyId",
                table: "Posts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            /*migrationBuilder.AddForeignKey(
                name: "FK_Posts_Posts_ReplyId",
                table: "Posts",
                column: "ReplyId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);*/
        }
    }
}
