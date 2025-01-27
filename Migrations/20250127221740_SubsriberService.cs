using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace let_em_cook.Migrations
{
    /// <inheritdoc />
    public partial class SubsriberService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserApplicationUser");

            migrationBuilder.CreateTable(
                name: "UserSubscriptions",
                columns: table => new
                {
                    SubscribedToId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubscriberId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubscriptionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptions", x => new { x.SubscribedToId, x.SubscriberId });
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_ApplicationUser_SubscribedToId",
                        column: x => x.SubscribedToId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_ApplicationUser_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_SubscriberId",
                table: "UserSubscriptions",
                column: "SubscriberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSubscriptions");

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
    }
}
