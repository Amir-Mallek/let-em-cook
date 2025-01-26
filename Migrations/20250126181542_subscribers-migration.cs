using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace let_em_cook.Migrations
{
    /// <inheritdoc />
    public partial class subscribersmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ApplicationUserApplicationUser",
                columns: table => new
                {
                    SubscribersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubscriptionsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserApplicationUser", x => new { x.SubscribersId, x.SubscriptionsId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserApplicationUser_ApplicationUser_SubscribersId",
                        column: x => x.SubscribersId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserApplicationUser_ApplicationUser_SubscriptionsId",
                        column: x => x.SubscriptionsId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserApplicationUser_SubscriptionsId",
                table: "ApplicationUserApplicationUser",
                column: "SubscriptionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserApplicationUser");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "ApplicationUser");
        }
    }
}
