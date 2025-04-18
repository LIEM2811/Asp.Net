using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lengocsiliem_2122110324.Migrations
{
    /// <inheritdoc />
    public partial class AddPay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Payments_PaymentsPaymentId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentsPaymentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentsPaymentId",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentId",
                table: "Orders",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Payments_PaymentId",
                table: "Orders",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Payments_PaymentId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "PaymentsPaymentId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentsPaymentId",
                table: "Orders",
                column: "PaymentsPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Payments_PaymentsPaymentId",
                table: "Orders",
                column: "PaymentsPaymentId",
                principalTable: "Payments",
                principalColumn: "PaymentId");
        }
    }
}
